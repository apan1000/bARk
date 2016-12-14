using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole;

public class Changer : MonoBehaviour
{
	Wasabimole.ProceduralTree.ProceduralTree tree;
	float timeBetween;
	Transform cam;

	// Use this for initialization
	void Start () {
		tree = GetComponent<Wasabimole.ProceduralTree.ProceduralTree>();
		cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > timeBetween && tree.growthPercent < 1) {
			tree.growthPercent += 0.001f;
			timeBetween = Time.time+0.02f;
		}
		
		// tree.growthPercent = Mathf.Clamp( Mathf.Abs( 0.005f * Vector3.Magnitude(cam.position - transform.position)),0f,1f);

	}
}
