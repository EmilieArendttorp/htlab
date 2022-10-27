using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct StaticGesture
{
    public string name;
    public List<Vector3> fingerData;
}

public class RecordGesture : MonoBehaviour
{
    HandInitializer handInitializer;
    DynamicGestureCreator dynamic;

    public float sphereInterval, sphereScale;
    public List<StaticGesture> R_Gestures;
    public List<StaticGesture> L_Gestures;

    public bool isLeftHand = true;
    [SerializeField]
    int fingerBoneIndex = 8;


    void Start()
    {
        dynamic = GetComponent<DynamicGestureCreator>();
        handInitializer = GetComponent<HandInitializer>();
    }

    void Update()
    {
        // Records the dynamic- AND static gesture.
        // The dynamic gesture is continously recorded while 'D' is pressed.
        // To actually save the gesture, you have to copy the list of values IN RUNTIME
        // so you can paste all the values in AFTER RUNTIME.
        if (handInitializer.isInitialized && handInitializer.debugMode && Input.GetKeyDown(KeyCode.D))
        {
            GestureSnapshot();
            dynamic.isRunning = true;
            dynamic.RecordSpheres(sphereInterval, handInitializer.fingerBonesRec[fingerBoneIndex], sphereScale);
        }

        // Stops the dynamic gesture recording
        if (handInitializer.isInitialized && handInitializer.debugMode && Input.GetKeyUp(KeyCode.D))
        {
            dynamic.isRunning = false;
        }

        // Records the static gesture - all the finger joints current position is saved in a list in the
        // "Gesture Manager" GameObject in the scene.
        // To actually save the gesture, you have to copy the values from the list IN RUNTIME
        // so you can paste all the values in AFTER RUNTIME.
        if (handInitializer.isInitialized && handInitializer.debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            GestureSnapshot();
        }
    }

    void GestureSnapshot()
    {
        // Creates new static gesture called g
        StaticGesture g = new StaticGesture();

        // Sets the name of the new gesture
        g.name = "New Gesture";

        // Creates a list of 3D vectors we use to save the individual bones' positions
        List<Vector3> fingerData = new List<Vector3>();

        foreach (var bone in handInitializer.fingerBonesRec)
        {
            // Adds each bone's position from the InverseTransformPoint(wrist) to the Vector3 list we just created
            fingerData.Add(handInitializer.recSkeleton.transform.InverseTransformPoint(bone.Transform.position));

        }
        // Sets gesture g's finger data equal to the fingerdata we just set above
        g.fingerData = fingerData;

        // Adds the gesture g to the list of gestures we loop through in Recognized()
        if (isLeftHand)
        {
            L_Gestures.Add(g);
        }
        else
        {
            R_Gestures.Add(g);
        }
    }
}
