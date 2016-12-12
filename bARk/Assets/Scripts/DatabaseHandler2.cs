using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class DatabaseHandler2 : MonoBehaviour {

    public Texture2D leafTex;

    DatabaseReference treeRef;

	// Use this for initialization
	void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        treeRef = reference.Child("Tree");
	}
	
    public void AddTreeToFirebase()
    {
        string key = treeRef.Push().Key;
        ARTree myTree = new ARTree("1","2","3","4");
        myTree.ConvertToString(leafTex);
        Dictionary<string, object> entryVal = myTree.ToDictionary();
        Dictionary<string, object> childUpdate = new Dictionary<string, object>();
        childUpdate["/Trees/" + key] = entryVal;
        treeRef.Parent.UpdateChildrenAsync(childUpdate);
    }
}
