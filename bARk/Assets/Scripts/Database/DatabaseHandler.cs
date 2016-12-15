using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using System;

public class DatabaseHandler : MonoBehaviour
{
    public delegate void DatabaseEvent(List<ARTree> allTrees);
    public static event DatabaseEvent TreesLoaded;
    public delegate void DatabaseEvent2(ARTree tree);
    public static event DatabaseEvent2 NewTreeAdded;

    public Texture2D leafTex;

    DatabaseReference rootRef;
    DatabaseReference treeRef;
    List<ARTree> allTrees;

    bool treesLoaded = false;
    DatabaseReference treeEvent;

	void Awake()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");
        rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        treeRef = rootRef.Child("Trees");

        // Listen for events 
        treeEvent = FirebaseDatabase.DefaultInstance.GetReference("Trees");

    }

    void Start()
    {
        //treeRef.LimitToFirst(1).ChildAdded += NewTree;
    }

    private void NewTree(object sender, ChildChangedEventArgs args)
    {
        Debug.Log("New tree found!!");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // ONLY ONE AT A TIME
        DataSnapshot snap = args.Snapshot;
        ARTree newTree = new ARTree(snap);
        NewTreeAdded(newTree);

    }

    private void TreeRemoved(object sender, ChildChangedEventArgs args)
    {
        Debug.Log("--Tree Removed!");

    }

    /// <summary>
    /// Adds a tree to firebase database
    /// </summary>
    public void AddTreeToFirebase(int seed, int maxNumVertices, int numberOfSides, float baseRadius, float radiusStep, float minimumRadius,
        float branchRoundness, float segmentLength, float twisting, float branchProbability, float growthPercent)
    {
        string key = treeRef.Push().Key;
        ARTree myTree = new ARTree(seed, maxNumVertices, numberOfSides, baseRadius, radiusStep, minimumRadius,
        branchRoundness, segmentLength, twisting, branchProbability, growthPercent, key);

        myTree.ConvertToString(leafTex);
        Dictionary<string, object> entryVal = myTree.ToDictionary();

        Dictionary<string, object> childUpdate = new Dictionary<string, object>();
        childUpdate["/Trees/" + key] = entryVal;
        treeRef.Parent.UpdateChildrenAsync(childUpdate);
    }

    /// <summary>
    /// Returns all trees found in database
    /// </summary>
    public List<ARTree> GetAllTrees()
    {
        allTrees = new List<ARTree>();
        treeEvent.GetValueAsync().ContinueWith(GetTrees);
        return allTrees;
    }

    /// <summary>
    /// Fetches all trees in databse an stores them in the databaseHandler object
    /// </summary>
    /// <param name="task"></param>
    private void GetTrees(Task<DataSnapshot> task)
    {
        allTrees = new List<ARTree>();
        if (task.IsFaulted)
            Debug.Log("Failed to Load");
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            foreach(var tree in snapshot.Children)
            {
                ARTree newTree = new ARTree(tree);
                allTrees.Add(newTree);
            }
            treesLoaded = true;
        }
    }

    // Event cal has to be called on main thread, to avoid 
    // issues in functions responding to this event.
    void Update()
    {
        if (treesLoaded)
        {
            TreesLoaded(allTrees);
            treesLoaded = false;

            // Start listenting after changes when trees has been loaded
            treeEvent.ChildAdded += NewTree;
            treeEvent.ChildRemoved += TreeRemoved;
        }
    }

    void OnDestroy()
    {
        treeRef.StartAt(allTrees.Count).ChildAdded -= NewTree;
        treeRef.ChildRemoved -= TreeRemoved;
    }
}
