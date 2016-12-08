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
		treeMaterial.mainTexture = GetComponent<Renderer>().sharedMaterial.mainTexture;
	}
}
