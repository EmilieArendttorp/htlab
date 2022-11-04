using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureFunctions : MonoBehaviour
{
    public MeshRenderer boxRend;

    
    // Between these comments may not be necessary
    HandInitializer handInitializer;
    RecognizeDynamicRHand recognizeDynamic_R;
    RecognizeDynamicLHand recognizeDynamic_L;

    private SphereDetector _sphereDetector;
    private Vector3 _lastHandPos = Vector3.zero;

    private void Start()
    {
        handInitializer = GetComponent<HandInitializer>();
        recognizeDynamic_R = GetComponent<RecognizeDynamicRHand>();
        recognizeDynamic_L = GetComponent<RecognizeDynamicLHand>();        
    }
    // Between these comments may not be necessary
    

    void M()
    {
        boxRend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    void RightHandCrawl()
    {
        var headset = FindObjectOfType<OVRCameraRig>().transform;
        var hand = GameObject.FindGameObjectWithTag("OVRHandR");
        //if (hand is null) return;
        
        var currentHandPos = hand.transform.position;
        var difference = _lastHandPos - currentHandPos;
        var headsetPosition = headset.position;
        var newPos = new Vector3(headsetPosition.x + difference.x, headsetPosition.y, headsetPosition.z + difference.z );
        headsetPosition = Vector3.MoveTowards(headsetPosition, newPos, 1f * Time.deltaTime);
        headset.position = headsetPosition;
        _lastHandPos = currentHandPos;
    }

    void LeftHandCrawl()
    {
        var headset = FindObjectOfType<OVRCameraRig>().transform;
        var hand = GameObject.FindGameObjectWithTag("OVRHandL");
        //if (hand is null) return;
        
        var currentHandPos = hand.transform.position;
        var difference = _lastHandPos - currentHandPos;
        var headsetPosition = headset.position;
        var newPos = new Vector3(headsetPosition.x + difference.x, headsetPosition.y, headsetPosition.z + difference.z );
        headsetPosition = Vector3.MoveTowards(headsetPosition, newPos, 1f * Time.deltaTime);
        headset.position = headsetPosition;
        _lastHandPos = currentHandPos;
    }
}
