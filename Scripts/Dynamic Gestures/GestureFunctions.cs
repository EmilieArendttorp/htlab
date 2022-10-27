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

    void GestureName_R()
    {
        // Name the function what you name the saved gesture and it will call this function
        // when the dynamic (or static, maybe) gesture is completed.
    }

    void GestureName_L()
    {
        // Name the function what you name the saved gesture and it will call this function
        // when the dynamic (or static, maybe) gesture is completed.
    }
}
