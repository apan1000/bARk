using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Vuforia;

public class LTree
{
    #region PRIVATE_MEMBER_VARIABLES
    private List<LTree> branches;
    private GameObject contents;
    private GameObject appearance;
    private LTree lParent;
    #endregion

    #region PUBLIC_MEMBER_VARIABLES
    public float length_decay = 0.8f;
    public float radius_decay = 0.7f;
    public float angle_deviation = 0.3f;
    public float minimum_branches = 1;
    public float maximum_branches = 3;
    public float minimum_radius = 0.1f;
    #endregion

    void createChildren()
    {
        float new_radius = appearance.transform.localScale.x * radius_decay;
        float new_length = appearance.transform.localScale.y * length_decay;
        if (new_radius < minimum_radius) return;
        branches = new List<LTree>();

        GameObject progenitor = new GameObject();
        progenitor.name = "Root for children";
        progenitor.transform.parent = contents.transform;
        progenitor.transform.localPosition = new Vector3(0, 0, 0);
        progenitor.transform.localEulerAngles = new Vector3(0, 0, 0);
        Debug.Log("the offset is " + appearance.transform.localPosition.y);
        progenitor.transform.Translate(0, 2 * appearance.transform.localPosition.y, 0);
        int num_children = (int)(UnityEngine.Random.value * (maximum_branches - minimum_branches) + minimum_branches);
        for (int i = 0; i < num_children; i++)
        {
            LTree child = new LTree();
            branches.Add(child);
            child.construct(null, progenitor, new_length, new_radius);
        }
    }

    public void pivot()
    {
        contents.transform.Rotate(0, .2f, 0);
    }

    public void do_rotate(float amt)
    {
        contents.transform.Rotate(0, 0, amt);
        if (branches == null) return;
        for (int i = 0; i < branches.Count; i++)
        {
            branches[i].do_rotate(amt);
        }
    }

    public void reset(float l, float r)
    {
        GameObject.Destroy(contents);
        branches = new List<LTree>();
        construct(null, null, l, r);
    }

	public void construct(Transform main, GameObject parentTree, float length, float radius)
	{
		contents = new GameObject();
		appearance = GameObject.CreatePrimitive(LTreeController.limbType);

		if (parentTree != null) {
			contents.transform.parent = parentTree.transform;
			contents.transform.localEulerAngles= new Vector3(0,0,0);
			contents.transform.Rotate(UnityEngine.Random.value*100-50,0,UnityEngine.Random.value*100-50);
		} else {
			if (main != null) {
				contents.transform.parent = main;
				contents.transform.localEulerAngles= new Vector3(0,0,0);
			}
		}

		contents.transform.localPosition = new Vector3 (0, 0, 0);
		contents.name = "Contents";

		appearance.transform.parent = contents.transform;
		appearance.transform.localPosition = new Vector3(0,0,0);
		appearance.transform.localEulerAngles= new Vector3(0,0,0);
		appearance.name = "Appearance";

		Vector3 scaleVector = new Vector3(radius, length, radius);
		appearance.transform.localScale = scaleVector;
		appearance.transform.Translate(0,0.5f*LTreeController.yScale*length,0);
        
        createChildren();
	}


	public IEnumerator grow()
	{
		float percent = 0;

		while (percent < 1) {

			for (int i = 0; i < branches.Count; i++) {
				branches [i].do_rotate(0.1f * Mathf.Cos(0.03f * percent));
				//branches [i].appearance.transform.localPosition = new Vector3 (0, 0.5f * LTreeController.yScale * percent, 0);
			}
			/*
			Vector3 scaleVector = new Vector3(radius, length* percent, radius);
			appearance.transform.localScale = scaleVector;
			appearance.transform.localPosition = new Vector3 (0, 0.5f * LTreeController.yScale * length * percent, 0);
			//appearance.transform.Translate(0,0.5f*LTreeController.yScale*length*percent,0);
				*/
			percent += 0.02f;
			yield return null;
		}

	}
}


public class LTreeController : MonoBehaviour, ITrackableEventHandler
{

    /*
    the LTree is an L-System-based tree object that creates a tree structure from simple rules.
    */
    public float initial_length = 1;
    public static PrimitiveType limbType = PrimitiveType.Cube;
    public static float yScale = 1f;
    public float initial_radius = 0.1f;
    public bool isTracking;

    private int t = 0;
    private LTree rootNode;
    
    private TrackableBehaviour mTrackableBehaviour;

    void Start()
    {
        rootNode = new LTree();
		rootNode.construct(transform, null, initial_length, initial_radius);
		//StartCoroutine (rootNode.grow ());

        mTrackableBehaviour = transform.parent.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
		
        t++;
        rootNode.do_rotate(0.1f * Mathf.Cos(0.03f * t));
        
        //rootNode.pivot();
    }

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            isTracking = true;
        }
        else
        {
            isTracking = false;
        }
    }
}

