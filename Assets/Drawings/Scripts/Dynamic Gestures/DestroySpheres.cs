using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySpheres : MonoBehaviour
{
    // This function is called to destroy potential other sphere paths 
    // when more than half of one sphere path has been completed
    public void DestroyOtherPaths(string pathName, string originSphereHand)
    {
        GameObject[] originSpheres = GameObject.FindGameObjectsWithTag(originSphereHand);

        for (int i = 0; i < originSpheres.Length; i++)
        {
            if (originSpheres[i].name != pathName)
            {
                Destroy(originSpheres[i]);
            }
        }
    }

    // This function is called to clear all sphere paths when no gesture is currently performed
    public void DestroyOriginSpheres(string originSphereHand)
    {
        GameObject[] originSpheres = GameObject.FindGameObjectsWithTag(originSphereHand);

        for (int i = 0; i < originSpheres.Length; i++)
        {
            Destroy(originSpheres[i]);
        }
    }
}
