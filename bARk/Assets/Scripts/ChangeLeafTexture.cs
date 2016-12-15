using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole.ProceduralTree;


public class ChangeLeafTexture : MonoBehaviour
{
    public void ChangeTexture()
    {
        Material newTex = transform.parent.GetComponent<ProceduralTree>().leafMaterial;

        foreach(Transform child in transform)
        {
            if (child == null) break;
            child.GetComponent<Renderer>().material.mainTexture = newTex.mainTexture;
        }
    }
}
