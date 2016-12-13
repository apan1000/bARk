using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole;

public class Changer : MonoBehaviour
{
    public DatabaseHandler2 database;
	Wasabimole.ProceduralTree.ProceduralTree tree;
	float timeBetween;
	Transform cam;

	// Use this for initialization
	void Start () {
		tree = GetComponent<Wasabimole.ProceduralTree.ProceduralTree>();
        StartCoroutine(SaveTree());
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

    IEnumerator SaveTree()
    {
        yield return new WaitForSeconds(3);
        database.AddTreeToFirebase(tree.Seed, tree.MaxNumVertices, tree.NumberOfSides, tree.BaseRadius,
            tree.RadiusStep, tree.MinimumRadius, tree.BranchProbability, tree.SegmentLength, tree.Twisting,
            tree.BranchProbability, tree.growthPercent);
    }
}
