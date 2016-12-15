using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole.ProceduralTree;

public class TreeDatabaseHandler : MonoBehaviour {
    [Header("Database")]
    public DatabaseHandler database;
    [Header("Tree")]
    public GameObject displayTrees;
    public GameObject rootTree;

    private ProceduralTree tree;

    // Use this for initialization
    void Start ()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Tree")
                tree = child.GetComponent<ProceduralTree>();
        }
        
        //StartCoroutine(SaveTree());
        database.GetAllTrees(); // STarts a thread in database that looks for trees
        DatabaseHandler.TreesLoaded += LoadedTrees; // Listen for when trees are found
    }

    /// <summary>
    /// When all trees has been found on the database, this gets called
    /// </summary>
    /// <param name="allTrees"></param>
    private void LoadedTrees(List<ARTree> allTrees)
    {
        bool showFirst = true;
        // Go through all trees and create dissabled gameobjects from them
        // store them as childs under displayTrees
        foreach (ARTree tree in allTrees)
        {
            GameObject g = Instantiate(rootTree) as GameObject;
            ProceduralTree script = g.GetComponent<ProceduralTree>();
            script.Seed = tree.seed;
            script.MaxNumVertices = tree.maxNumVertices;
            script.NumberOfSides = tree.numberOfSides;
            script.BaseRadius = tree.baseRadius;
            script.RadiusStep = tree.radiusStep;
            script.MinimumRadius = tree.minimumRadius;
            script.BranchRoundness = tree.branchRoundness;
            script.SegmentLength = tree.segmentLength;
            script.Twisting = tree.twisting;
            script.BranchProbability = tree.branchProbability;
            script.growthPercent = 1.0f;// tree.growthPercent;

            // Set correct leaf-texture
            tree.ConvertToTexture();
            Material m = new Material(Shader.Find("Custom/LeafShader"));
            m.mainTexture = tree.leafTexture;
            script.leafMaterial = m;

            g.transform.parent = displayTrees.transform;
            g.transform.localScale = new Vector3(.2f, .2f, .2f);
            g.SetActive(showFirst);

            // Change texture on leafs
            GameObject leaf = g.transform.GetChild(0).gameObject;
            leaf.GetComponent<ChangeLeafTexture>().ChangeTexture();

            showFirst = false;  
        }
    }

    /// <summary>
    /// Saves the newly created tree to the database.
    /// </summary>
    public void SaveTree()
    {
        Debug.Log("Tree saved!");
        database.AddTreeToFirebase(tree.Seed, tree.MaxNumVertices, tree.NumberOfSides, tree.BaseRadius,
           tree.RadiusStep, tree.MinimumRadius, tree.BranchProbability, tree.SegmentLength, tree.Twisting,
           tree.BranchProbability, tree.growthPercent);
    }

    private void OnDestroy()
    {
        DatabaseHandler.TreesLoaded -= LoadedTrees;
    }
}