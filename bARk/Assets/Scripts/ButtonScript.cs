using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	public Material TreeMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetTreeTexture() {
		TreeMaterial.mainTexture = GetComponent<Renderer>().sharedMaterial.mainTexture;
	}
}
