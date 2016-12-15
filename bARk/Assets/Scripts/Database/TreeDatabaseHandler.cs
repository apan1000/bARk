using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole.ProceduralTree;

public class TreeDatabaseHandler : MonoBehaviour
{
    public delegate void TreeDestroyedInScene();
    public static event TreeDestroyedInScene TreeDestroyed;

    [Header("Database")]
    public DatabaseHandler database;
    [Header("Tree")]
    public GameObject displayTrees;
    public GameObject rootTree;
    public Texture2D[] barkTextures;

    private ProceduralTree tree;
    private Renderer treeMaterial;
    private List<ARTree> treesToAdd;
    private List<ARTree> treesToRemove;
    private bool firstTree = true;

    // Use this for initialization
    void Start ()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Tree")
            {
                tree = child.GetComponent<ProceduralTree>();
                treeMaterial = child.GetComponent<Renderer>();
            }
        }
        treesToAdd = new List<ARTree>();
        treesToRemove = new List<ARTree>();
        DatabaseHandler.NewTreeAdded += AddTreeToAdd;
        DatabaseHandler.NewTreeToRemove += AddTreeToRemove;
    }

    /// <summary>
    /// Saves the newly created tree to the database.
    /// </summary>
    public void SaveTree()
    {


        string materialName = treeMaterial.sharedMaterial.mainTexture.name;
        database.AddTreeToFirebase(tree.Seed, tree.MaxNumVertices, tree.NumberOfSides, tree.BaseRadius,
           tree.RadiusStep, tree.MinimumRadius, tree.BranchProbability, tree.SegmentLength, tree.Twisting,
           tree.BranchProbability, tree.growthPercent, materialName);
    }

    /// <summary>
    /// Checks if new tree exists already. If not, add it to the scene.
    /// </summary>
    private void AddTreeToAdd(ARTree newTree)
    {
        treesToAdd.Add(newTree);
    }

    private void AddTreeToRemove(ARTree removeTree)
    {
        treesToRemove.Add(removeTree);
    }

    /// <summary>
    /// Check if any new trees are ready to be added to the scene.
    /// </summary>
    void Update()
    {
        if(treesToAdd.Count > 0)
        {
            ARTree newTree = treesToAdd[0]; // Two step Pop
            treesToAdd.RemoveAt(0);

            bool addTree = true;
            foreach (Transform child in displayTrees.transform)
            {
                string timeStampEx = child.gameObject.GetComponent<TimeStampHolder>().TimeStamp;

                // Do not add tree to scene if it already exists
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
                AddTreeToScene(g);
            }
        }

        if(treesToRemove.Count > 0)
        {
            ARTree removeTree = treesToRemove[0];
            treesToRemove.RemoveAt(0);

            foreach (Transform child in displayTrees.transform)
            {
                string timeStampEx = child.gameObject.GetComponent<TimeStampHolder>().TimeStamp;
                if(removeTree.timeStamp == timeStampEx)
                {
                    Debug.Log("Child Removed");
                    Destroy(child.gameObject);
                    TreeDestroyed();
                    break;
                }
            }
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

        // Set the propper tree texture
        Renderer treeR = g.GetComponent<Renderer>();
        string textureName = tree.materialName; // tree.materialName is textureName;
        if (textureName == "bark3") // Oak
        {
            Debug.Log("BARK3");
            treeR.material.mainTexture = barkTextures[0];
        }
        else if (textureName == "bark1") // Rainbow
        {
            Debug.Log("BARK1");
            treeR.material.mainTexture = barkTextures[1];
        }
        else // Birch
        {
            Debug.Log("BIRCH!!!");
            g.AddComponent<ProceduralBark>();
            ProceduralBark b = g.GetComponent<ProceduralBark>();
            b.freq = 0.08f;
            b.lacunarity = 3.38f;
            b.gain = -0.78f;
            b.GenerateTexture();
        }
        return g;
    }

    /// <summary>
    /// Adds a tree GameObject to the scene.
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="enabled"></param>
    private void AddTreeToScene(GameObject g)
    {
        g.transform.parent = displayTrees.transform;
        g.transform.localScale = new Vector3(.2f, .2f, .2f);
        g.SetActive(firstTree);
        firstTree = false;
    }
}