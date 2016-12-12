using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlattScript : MonoBehaviour {

    public float growthtime = 0.1f;

	// Use this for initialization
	void Start () {
        Spawn();
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.x < 1.0f)
        {
        //    transform.localScale = new Vector3(scale, scale/1000, scale);
            Vector3 scale = new Vector3(Time.deltaTime * growthtime, Time.deltaTime * growthtime/100, Time.deltaTime * growthtime);
            transform.localScale += scale;
        }
	}

    public void Spawn()
    {
        transform.localScale = Vector3.zero;
    }
}
