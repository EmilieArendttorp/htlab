using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DynamicSphere
{
    public string name, gesture_trigger;
    public List<Vector3> spherePos;
    public int fingerBoneNr;
}

public class DynamicGestureCreator : MonoBehaviour
{
    public GameObject gestureSphere, gesturePath;

    public List<DynamicSphere> dynamicGestures;

    public bool isRunning = false;


    // This funtion is called from "RecordGesture.cs" and records a dynamic gesture -
    // runs CreateSpheres() as long as the 'D' key is held down.
    public void RecordSpheres(float interval, OVRBone fingerBone, float sphereScale)
    {
        StartCoroutine(CreateSpheres(interval, fingerBone, sphereScale));
    }

    // This function runs as a separate thread i.e. IEnumerator...
    // CreateSpheres() is responsible for instantiating spheres while recording a new dynamic gesture.
    // The dynamic gesture(s) can be found in a list on the "Gesture Manager" GameObject in the scene.
    IEnumerator CreateSpheres(float interval, OVRBone fingerBone, float sphereScale)
    {
        DynamicSphere spherePath = new DynamicSphere();
        spherePath.name = "New sphere path";
        spherePath.gesture_trigger = "New gesture trigger";
        spherePath.fingerBoneNr = fingerBone.ParentBoneIndex;

        GameObject emptyGesturePath = Instantiate(gesturePath, fingerBone.Transform.position, Quaternion.identity);

        List<Vector3> SphereList = new List<Vector3>();


        int sphereNumber = 0;
        while (isRunning)
        {
            GameObject sphere = Instantiate(gestureSphere, fingerBone.Transform.position, Quaternion.identity, emptyGesturePath.transform);
            sphere.transform.localScale = sphere.transform.localScale * sphereScale;
            sphere.name = "" + sphereNumber;
            SphereList.Add(sphere.transform.localPosition);
            sphereNumber++;
            yield return new WaitForSeconds(interval);
        }

        spherePath.spherePos = SphereList;
        dynamicGestures.Add(spherePath);

        yield return null;
    }


}
