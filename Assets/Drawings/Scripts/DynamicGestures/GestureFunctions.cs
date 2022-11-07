using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GestureFunctions : MonoBehaviour
{
    [SerializeField] float crawlSpeed = 1f;
    public MeshRenderer boxRend;

    
    // Between these comments may not be necessary
    HandInitializer handInitializer;
    RecognizeDynamicRHand recognizeDynamic_R;
    RecognizeDynamicLHand recognizeDynamic_L;
    Transform headset;
    GameObject rightHand;
    GameObject leftHand;
    
    private RecognizeStaticLHand recStaticL;
    private RecognizeStaticRHand recStaticR;

    private SphereDetector _sphereDetector;
    private Vector3 _lastLeftHandPos = Vector3.zero;
    private Vector3 _lastRightHandPos = Vector3.zero;

    private void Start()
    {
        handInitializer = GetComponent<HandInitializer>();
        recognizeDynamic_R = GetComponent<RecognizeDynamicRHand>();
        recognizeDynamic_L = GetComponent<RecognizeDynamicLHand>();
        headset = FindObjectOfType<OVRCameraRig>().transform;
        rightHand = GameObject.FindGameObjectWithTag("OVRHandR");
        leftHand = GameObject.FindGameObjectWithTag("OVRHandL");

        recStaticL = GetComponent<RecognizeStaticLHand>();
        recStaticR = GetComponent<RecognizeStaticRHand>();
    }
    // Between these comments may not be necessary

    private void Update()
    {
        if (recStaticL.currentGesture_L.name == null) _lastLeftHandPos = Vector3.zero;
        if (recStaticR.currentGesture_R.name == null) _lastRightHandPos = Vector3.zero;
    }

    void M()
    {
        boxRend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    void RightHandCrawl()
    {
        var currentHandPos = rightHand.transform.position;
        if (_lastRightHandPos.Equals(Vector3.zero)) _lastRightHandPos = currentHandPos;
        
        var difference = (_lastRightHandPos - currentHandPos) * crawlSpeed;
        headset.position += new Vector3(difference.x, 0, difference.z);
        _lastRightHandPos = currentHandPos + difference;
    }

    void LeftHandCrawl()
    {
        var currentHandPos = leftHand.transform.position;
        if (_lastLeftHandPos.Equals(Vector3.zero)) _lastLeftHandPos = currentHandPos;
        
        var difference = (_lastLeftHandPos - currentHandPos) * crawlSpeed;
        headset.position += new Vector3(difference.x, 0, difference.z);
        _lastLeftHandPos = currentHandPos + difference;
    }
}
