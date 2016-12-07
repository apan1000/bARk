using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole;


public class Changer : MonoBehaviour {

	Wasabimole.ProceduralTree.ProceduralTree tree;
	float timeBetween;

	// Use this for initialization
	void Start () {
		tree = GetComponent<Wasabimole.ProceduralTree.ProceduralTree>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > timeBetween && tree.GrowthMultiplier < 1) {
			tree.GrowthMultiplier += 0.01f;
			timeBetween = Time.time+0.02f;
		}
	}
}
