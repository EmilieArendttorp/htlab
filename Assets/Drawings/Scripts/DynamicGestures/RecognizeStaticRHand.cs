using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognizeStaticRHand : MonoBehaviour
{
    RecordGesture recordGesture;
    HandInitializer handInitializer;

    public float threshold = 0.045f;

    public StaticGesture currentGesture_R;

    // Start is called before the first frame update
    void Start()
    {
        recordGesture = GetComponent<RecordGesture>();
        handInitializer = GetComponent<HandInitializer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets currentGesture_L to the current static gesture performed, if debugmode is false and the hands are initialized
        if (handInitializer.isInitialized && !handInitializer.debugMode)
        {
            currentGesture_R = RecognizedRight();

            //Debug.Log("I recognised the right hand doing: " + currentGesture_R.name + "!");
        }
    }

    // Checks the distance between live finger joint positions and saved static gesture finger joint positions.
    // Returns a StaticGesture if the distance is below a set threshold -- 0.045 is pretty decent.
    StaticGesture RecognizedRight()
    {
        StaticGesture currentGesture = new StaticGesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in recordGesture.R_Gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < handInitializer.fingerBonesRightH.Count; i++)
            {
                Vector3 currentData = handInitializer.rightHandSkeleton.transform.InverseTransformPoint(handInitializer.fingerBonesRightH[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }

        }
        return currentGesture;
    }
}
