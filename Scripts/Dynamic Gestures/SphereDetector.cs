using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDetector : MonoBehaviour
{
    DynamicGestureCreator dynamicGestureCreator;
    GestureFunctions GestFunc;
    DestroySpheres destroySpheres;
    RecognizeDynamicRHand recognizeDynamic_R;
    RecognizeDynamicLHand recognizeDynamic_L;
    HandInitializer handInitializer;

    GameObject gestureManager;

    List<OVRBone> fingerBones;
    string originSphereHand;

    DynamicSphere currentPath;
    int whichSphere = 0;
    public float indexOriginThreshold = 0.2f;
    public int sphereFingerIndex = 8;
    float pathCompletion; // Could be used to progressively fire script (lerp) in GestureFunction.cs
    bool pathCompleted = false;
    bool isRightHand;

    private void Start()
    {
        gestureManager = GameObject.FindGameObjectWithTag("GestureManager");
        dynamicGestureCreator = gestureManager.GetComponent<DynamicGestureCreator>();
        GestFunc = gestureManager.GetComponent<GestureFunctions>();
        destroySpheres = gestureManager.GetComponent<DestroySpheres>();
        recognizeDynamic_R = gestureManager.GetComponent<RecognizeDynamicRHand>();
        recognizeDynamic_L = gestureManager.GetComponent<RecognizeDynamicLHand>();
        handInitializer = gestureManager.GetComponent<HandInitializer>();

        if (this.transform.IsChildOf(GameObject.FindGameObjectWithTag("OVRHandL").transform))
        {
            isRightHand = false;
            fingerBones = handInitializer.fingerBonesLeftH;
            originSphereHand = "OriginSphere_L";
            Debug.Log("The index sphere is on the left hand");
        }
        else if (this.transform.IsChildOf(GameObject.FindGameObjectWithTag("OVRHandR").transform))
        {
            isRightHand = true;
            fingerBones = handInitializer.fingerBonesRightH;
            originSphereHand = "OriginSphere_R";
            Debug.Log("The index sphere is on the right hand");
        }

        //if (this.transform.parent.tag == "OVRHandL")
        //{
        //    fingerBones = handInitializer.fingerBonesLeftH;
        //}
        //else if (this.transform.parent.tag == "OVRHandR")
        //{
        //    fingerBones = handInitializer.fingerBonesRightH;
        //}
    }

    private void Update()
    {
        if (isRightHand)
        {
            if (recognizeDynamic_R.pathsInstantiated_R)
            {
                MoveOriginSpheres(originSphereHand);
            }
            else
            {
                ResetSphereCount();
            }
        }
        else
        {
            if (recognizeDynamic_L.pathsInstantiated_L)
            {
                MoveOriginSpheres(originSphereHand);
            }
            else
            {
                ResetSphereCount();
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        SphereCollision(other, originSphereHand);
    }

    void SphereCollision(Collider other, string originSphereHand)
    {
        if (other.tag == "GestureSphere" && other.transform.IsChildOf(GameObject.FindGameObjectWithTag(originSphereHand).transform))    
        {
            string sphereName = other.name.Substring(0, other.name.Length - 2);
            int sphereNo = int.Parse(other.name.Substring(other.name.Length - 2));

            if (sphereNo == whichSphere + 1)
            {
                currentPath = dynamicGestureCreator.dynamicGestures.Find(x => x.name.Contains(sphereName));
                if (sphereNo == currentPath.spherePos.Count / 2) // Half the lenght of the current path
                {
                    destroySpheres.DestroyOtherPaths(sphereName, originSphereHand); //function call to destroy the other paths not used
                }
                //if (sphereNo < currentPath.spherePos.Count - 1)
                //{
                //    GestFunc.Invoke(sphereName, 0); // allows GestureFunction.cs to fire function incrementally
                //}
                whichSphere++;
                pathCompletion = (float)whichSphere / currentPath.spherePos.Count;
                if (sphereNo == currentPath.spherePos.Count - 1)
                {
                    pathCompleted = true; // Allows GestureFunction.cs to fire script when the path is fully completed
                    try
                    {
                        GestFunc.Invoke(sphereName, 0);
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("After path completion, could not invoke GestureFunction." + sphereName);
                        throw;
                    }
                    Invoke(nameof(ResetSphereCount), .1f);
                }
                Destroy(other.gameObject); // Destroys the sphere it enters, that way we won't enter it again by mistake
            }
        }
    }

    void MoveOriginSpheres(string originSphereHand)
    {
        GameObject[] originSpheres = GameObject.FindGameObjectsWithTag(originSphereHand);


        for (int i = 0; i < originSpheres.Length; i++)
        {
            float indexOriginDistance = Vector3.Distance(originSpheres[i].transform.position, fingerBones[sphereFingerIndex].Transform.position);
            if (indexOriginDistance >= indexOriginThreshold)
            {
                originSpheres[i].transform.position = fingerBones[sphereFingerIndex].Transform.position;
                //originSpheres[i].transform.rotation = Camera.main.transform.rotation;
            }
        }

        // We need to check in Update() whether the current distance between the origin sphere and the index tip
        // is greater than a threshold. If it is, we want to set the positon of the originSphere
        // to the position of the index tip, and set the rotation equal to the camera's rotation.

        // We need a reference to the fingerbone, the originSphere and the camera.
    }

    void ResetSphereCount()
    {
        whichSphere = 0;
        pathCompleted = false;
    }
}
