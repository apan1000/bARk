using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
	
	private Material treeMaterial;

	// Use this for initialization
	void Start () {
		treeMaterial = GameObject.FindGameObjectWithTag("Tree").GetComponent<Renderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetTreeTexture() {
		// Set emissionMap since albedo/mainTex won't show
		treeMaterial.SetTexture("_EmissionMap", GetComponent<Renderer>().sharedMaterial.mainTexture);
	}
}
