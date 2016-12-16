using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAudio : MonoBehaviour {

    public GameObject treeObject;
    private Wasabimole.ProceduralTree.ProceduralTree tree;

    public AudioClip[] treeSounds;
    public float growthPlayInterval = 0.03f;
    private float lastRecordedGrowth = 0;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        tree = treeObject.GetComponent<Wasabimole.ProceduralTree.ProceduralTree>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (tree.growthPercent >= lastRecordedGrowth + growthPlayInterval && tree.growthPercent <= lastRecordedGrowth + 3 * growthPlayInterval) {
            int index = Random.Range(0, treeSounds.Length);
            print(index);
            audioSource.clip = treeSounds[index];
            audioSource.Play();
            lastRecordedGrowth = tree.growthPercent;
        }
	}
}
