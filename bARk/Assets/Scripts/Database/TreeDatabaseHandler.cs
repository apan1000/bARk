using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole.ProceduralTree;

public class TreeDatabaseHandler : MonoBehaviour
{
    [Header("Database")]
    public DatabaseHandler database;
    [Header("Tree")]
    public GameObject displayTrees;
    public GameObject rootTree;

    private ProceduralTree tree;
    private List<ARTree> newTreesToAdd;


    // Use this for initialization
    void Start ()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Tree")
                tree = child.GetComponent<ProceduralTree>();
        }
        newTreesToAdd = new List<ARTree>();
        database.GetAllTrees(); // STarts a thread in database that looks for trees
        DatabaseHandler.TreesLoaded += LoadedTrees; // Listen for when trees are found
        DatabaseHandler.NewTreeAdded += AddTreeToAdd;
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

    /// <summary>
    /// Checks if new tree exists already. If not, add it to the scene.
    /// </summary>
    private void AddTreeToAdd(ARTree newTree)
    {
        newTreesToAdd.Add(newTree);
    }

    /// <summary>
    /// Check if any new trees are ready to be added to the scene.
    /// </summary>
    void Update()
    {
        if(newTreesToAdd.Count > 0)
        {
            ARTree newTree = newTreesToAdd[0]; // Two step Pop
            newTreesToAdd.RemoveAt(0);

            bool addTree = true;
            foreach (Transform child in displayTrees.transform)
            {
                string timeStampEx = child.gameObject.GetComponent<TimeStampHolder>().TimeStamp;

                if (newTree.timeStamp == timeStampEx)
                {
                    Debug.Log("Tree excluded");
                    addTree = false;
                    break;
                }
            }
            if (addTree)
            {
                Debug.Log("Tree added!");
                GameObject g = CreateTreeGameobject(newTree);
                AddTreeToScene(g, false);
            }
        }
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
            GameObject g = CreateTreeGameobject(tree);
            AddTreeToScene(g, showFirst);
            showFirst = false;  
        }
    }

    /// <summary>
    /// Creates a GameObject from an ARTree
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private GameObject CreateTreeGameobject(ARTree tree)
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

        // Set correct leaf-material
        tree.ConvertToTexture();
        Material m = new Material(Shader.Find("Custom/LeafShader"));
        m.mainTexture = tree.leafTexture;
        script.leafMaterial = m;

        // Change texture on leafs
        GameObject leaf = g.transform.GetChild(0).gameObject;
        leaf.GetComponent<ChangeLeafTexture>().ChangeTexture();

        g.AddComponent<TimeStampHolder>().TimeStamp = tree.timeStamp;

        return g;
    }

    /// <summary>
    /// Adds a tree GameObject to the scene.
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="enabled"></param>
    private void AddTreeToScene(GameObject g, bool enabled)
    {
        g.transform.parent = displayTrees.transform;
        g.transform.localScale = new Vector3(.2f, .2f, .2f);
        g.SetActive(enabled);
    }

    private void OnDestroy()
    {
        DatabaseHandler.TreesLoaded -= LoadedTrees;
    }
}