using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
	
	public float growStyle = 0.1f; // Just branch probability for now
	private Wasabimole.ProceduralTree.ProceduralTree treeScript;
	private Material treeMaterial;

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform.parent.transform.parent.transform) { // in [imageTarget]
			if(child.tag == "Tree") {
				treeScript = child.GetComponent<Wasabimole.ProceduralTree.ProceduralTree>(); 
				treeMaterial = child.GetComponent<Renderer>().sharedMaterial;
			}	
         }
		// treeMaterial = GameObject.FindGameObjectWithTag("Tree").GetComponent<Renderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetTreeTexture() {
		// Set emissionMap since albedo/mainTex won't show
		treeMaterial.SetTexture("_EmissionMap", GetComponent<Renderer>().sharedMaterial.mainTexture);
	}

	public void SetGrowStyle() {
		treeScript.GrowthMultiplier = 0.1f;
		treeScript.BranchProbability = growStyle;
	}
}
