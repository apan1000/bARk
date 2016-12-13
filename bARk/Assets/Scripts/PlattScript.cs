using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlattScript : MonoBehaviour {

    public float growthtime = 0.1f;
    public float targetScale = 20.0f;

	// Use this for initialization
	void Start () {
        Spawn();
    }

    void OnEnable()
    {
        Spawn();
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.x < targetScale)
        {
        //    transform.localScale = new Vector3(scale, scale/1000, scale);
            Vector3 scale = new Vector3(Time.deltaTime * growthtime, Time.deltaTime * growthtime/100, Time.deltaTime * growthtime);
            transform.localScale += scale;
        }
        else if (transform.localScale.x > targetScale)
        {
            transform.localScale = new Vector3(targetScale, targetScale / 100, targetScale);
        }
	}

    public void Spawn()
    {
        transform.localScale = Vector3.zero;
    }
}
