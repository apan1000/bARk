using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

public class DatabaseHandler2 : MonoBehaviour
{
    public Texture2D leafTex;

    DatabaseReference treeRef;
    List<ARTree> myTrees;

	void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        myTrees = new List<ARTree>();
        treeRef = reference.Child("Tree");
        treeRef.GetValueAsync().ContinueWith(ReadAllTrees);
	}
	
    public void AddTree()
    {
        AddTreeToFirebase(12, 1231, 2, 1.3f, 1.5f, 0.3f, 2.4f, 12.1f, 12.2f, 0.4f, 0.5f);
    }

    /// <summary>
    /// Adds a tree to firebase database
    /// </summary>
    private void AddTreeToFirebase(int seed, int maxNumVertices, int numberOfSides, float baseRadius, float radiusStep, float minimumRadius,
        float branchRoundness, float segmentLength, float twisting, float branchProbability, float growthPercent)
    {
        string key = treeRef.Push().Key;
        ARTree myTree = new ARTree(seed, maxNumVertices, numberOfSides, baseRadius, radiusStep, minimumRadius,
        branchRoundness, segmentLength, twisting, branchProbability, growthPercent);

        //ARTreeTest myTree = new ARTreeTest(13.5f);

        myTree.ConvertToString(leafTex);
        Dictionary<string, object> entryVal = myTree.ToDictionary();

        Dictionary<string, object> childUpdate = new Dictionary<string, object>();
        childUpdate["/Trees/" + key] = entryVal;
        treeRef.Parent.UpdateChildrenAsync(childUpdate);
    }

    private void ReadAllTrees(Task<DataSnapshot> task)
    {
        if (task.IsFaulted)
            Debug.Log("Failed to Load");
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            foreach(var tree in snapshot.Children)
            {

            }
        }
    }
}
