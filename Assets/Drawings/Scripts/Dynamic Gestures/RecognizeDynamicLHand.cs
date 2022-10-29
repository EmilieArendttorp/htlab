using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognizeDynamicLHand : MonoBehaviour
{

    // 1. Static gesture trigger (RecognizeRightHGesture.cs)
    // A static gesture instantiates the paths with that gesture trigger.
    // 2. Instatiate dynamic gesture spheres - can have multiple paths
    // The spheres for each path are instatiated.
    // 3. Check which path is chosen
    // Check which sphere_0 is hit.
    // If sphere_1|2 from the same path is hit, discard the other paths
    // If all spheres from the same path is hit, trigger a function.
    // 4. Check if the spheres are hit in the correct order





    DynamicGestureCreator dynamic;
    RecognizeStaticLHand leftHGesture;
    HandInitializer handInitializer;
    RecordGesture recordGesture;
    DestroySpheres destroySpheres;
    GestureFunctions gestureFunctions;

    public GameObject indexSphere;
    public GameObject indexSphereL;


    public List<GameObject> spherePathList_L;

    [SerializeField]
    int indexSphereBone = 8; // This index is the joint on which the sphere detector will be instantiated. 8 is the base of the middle finger, 9 is the index finger tip.
    [SerializeField]
    float indexSphereScale = .1f;
    [SerializeField]
    float gestureResetDelay = .5f;

    public bool pathsInstantiated_L = false;
    bool isReset = true;
    bool sphereDetectorInstantiated = false;
    float timer = 0f;

    string palmOrientation;

    StaticGesture previousGesture_L;

    // Start is called before the first frame update
    void Start()
    {
        dynamic = GetComponent<DynamicGestureCreator>();
        leftHGesture = GetComponent<RecognizeStaticLHand>();
        handInitializer = GetComponent<HandInitializer>();
        recordGesture = GetComponent<RecordGesture>();
        destroySpheres = GetComponent<DestroySpheres>();
        gestureFunctions = GetComponent<GestureFunctions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!handInitializer.debugMode)
        {
            if (handInitializer.isInitialized && leftHGesture.currentGesture_L.name != null && !pathsInstantiated_L)
            {
                Vector3 palmDirection = handInitializer.fingerBonesLeftH[9].Transform.up;
                palmOrientation = SnapDirection(palmDirection);
                //Debug.Log("The palm's orientation is currently: " + palmOrientation);

                if (isReset)
                {
                    isReset = false;
                    timer = 0;
                }
                // If there is no sphereDetector and the orientation of the palm is down (orientation optional), 
                // instantiate sphereDecector and sphere paths
                if (!sphereDetectorInstantiated && palmOrientation == "down")
                {
                    InstantiateDetector(indexSphereBone, indexSphereScale);
                    InstantiateDynamicLeftHGestures();
                }
            }

            // Tries to invoke the name of the current gesture being performed
            if (leftHGesture.currentGesture_L.name != null && leftHGesture.currentGesture_L.name != previousGesture_L.name)
            {
                try
                {
                    gestureFunctions.Invoke(leftHGesture.currentGesture_L.name, 0);
                }
                catch (System.Exception)
                {
                    Debug.Log("Could not invoke: " + leftHGesture.currentGesture_L.name);
                    throw;
                }

            }

            // Clean up spheres when current gesture = null (no gesture recognized)
            if (leftHGesture.currentGesture_L.name == null && !isReset)
            {
                timer += Time.deltaTime;
                if (timer >= gestureResetDelay)
                {
                    destroySpheres.DestroyOriginSpheres("OriginSphere_L");
                    DestroyDetector();
                    spherePathList_L.Clear();
                    pathsInstantiated_L = false;
                    sphereDetectorInstantiated = false;
                    isReset = true;
                }

            }

            // Update previous gesture
            if (previousGesture_L.name != leftHGesture.currentGesture_L.name)
            {
                previousGesture_L = leftHGesture.currentGesture_L;
            }
        }
        
    }


    void InstantiateDynamicLeftHGestures()
    {
        DynamicSphere thisGesture;
        int paths = 0;
        for (int i = 0; i < dynamic.dynamicGestures.Count; i++)
        {
            if (dynamic.dynamicGestures[i].gesture_trigger == leftHGesture.currentGesture_L.name)
            {
                thisGesture = dynamic.dynamicGestures[i];
                thisGesture.spherePos[0] = handInitializer.fingerBonesLeftH[dynamic.dynamicGestures[i].fingerBoneNr].Transform.position;
                GameObject originSphere = Instantiate(dynamic.gestureSphere, thisGesture.spherePos[0], Quaternion.identity);
                originSphere.transform.localScale = originSphere.transform.localScale * recordGesture.sphereScale;
                originSphere.name = thisGesture.name;
                originSphere.tag = "OriginSphere_L";
                originSphere.gameObject.GetComponent<MeshRenderer>().enabled = false;
                for (int j = 1; j < thisGesture.spherePos.Count; j++)
                {
                    Vector3 position = thisGesture.spherePos[j] + handInitializer.fingerBonesLeftH[dynamic.dynamicGestures[i].fingerBoneNr].Transform.position;
                    GameObject sphere = Instantiate(dynamic.gestureSphere, position, Quaternion.identity, originSphere.transform);
                    if (j < 10)
                    {
                        sphere.name = thisGesture.name + "0" + j;
                    }
                    else sphere.name = thisGesture.name + j;

                    spherePathList_L.Add(sphere);
                }
                //originSphere.transform.rotation = Camera.main.transform.rotation;
                paths++;
            }
        }
        if (paths > 0)
        {
            pathsInstantiated_L = true;
        }
    }

    void InstantiateDetector(int fingerBone, float detectorScale)
    {
        // Instantiate a sphere at right hand index fingertip, used to check collision with gesture path spheres
        indexSphereL = Instantiate(indexSphere, handInitializer.fingerBonesLeftH[fingerBone].Transform.position, Quaternion.identity, handInitializer.fingerBonesLeftH[fingerBone].Transform);
        indexSphereL.transform.localScale = indexSphereL.transform.localScale * detectorScale;
        sphereDetectorInstantiated = true;
    }

    void DestroyDetector()
    {
        Destroy(indexSphereL);
    }


    string SnapDirection(Vector3 direction)
    {
        
        Vector3 right = Camera.main.transform.right;
        Vector3 left = -Camera.main.transform.right;
        Vector3 up = new Vector3(0, 1, 0);
        Vector3 down = new Vector3(0, -1, 0);
        Vector3 forward = Camera.main.transform.forward;
        Vector3 backward = -Camera.main.transform.forward;

        Vector3[] axes = { right, left, up, down, forward, backward};
        string[] directionName = { "right", "left", "up", "down", "forward", "backward"};

        Vector3 result = axes[0];
        string resultName = directionName[0];

        for (int i = 1; i < axes.Length; i++)
        {
            float currentBest = Vector3.Angle(direction, result);
            float current = Vector3.Angle(direction, axes[i]);
            if (current < currentBest)
            {
                result = axes[i];
                resultName = directionName[i];
            }
        }

        return resultName;
    }




}
