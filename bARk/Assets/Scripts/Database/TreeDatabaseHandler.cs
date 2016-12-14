using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDatabaseHandler : MonoBehaviour {

    public DatabaseHandler database;

    private Wasabimole.ProceduralTree.ProceduralTree tree;

    // Use this for initialization
    void Start ()
    {
        foreach (Transform child in transform.parent.transform.parent.transform)
        {
            if (child.tag == "Tree")
                tree = child.GetComponent<Wasabimole.ProceduralTree.ProceduralTree>();
        }
        
        //StartCoroutine(SaveTree());
        database.GetAllTrees();
        DatabaseHandler.TreesLoaded += LoadedTrees;
    }

    private void LoadedTrees(List<ARTree> allTrees)
    {
        Debug.Log("Nr of trees: " + allTrees.Count);
    }

    //IEnumerator SaveTree()
    //{
    //    yield return new WaitForSeconds(1);
    //    database.AddTreeToFirebase(tree.Seed, tree.MaxNumVertices, tree.NumberOfSides, tree.BaseRadius,
    //        tree.RadiusStep, tree.MinimumRadius, tree.BranchProbability, tree.SegmentLength, tree.Twisting,
    //        tree.BranchProbability, tree.growthPercent);
    //}

    public void SaveTree()
    {
        database.AddTreeToFirebase(tree.Seed, tree.MaxNumVertices, tree.NumberOfSides, tree.BaseRadius,
           tree.RadiusStep, tree.MinimumRadius, tree.BranchProbability, tree.SegmentLength, tree.Twisting,
           tree.BranchProbability, tree.growthPercent);
    }

    private void OnDestroy()
    {
        DatabaseHandler.TreesLoaded -= LoadedTrees;
    }
}
