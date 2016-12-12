using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// Firebase can only be accessed when running the application on a phone
// Error will occur if you try to access it in play mode
public class DatabaseHandler : MonoBehaviour
{
    public Text info;
    public Button[] buttons;

    private List<ARTree> m_theTrees;
    private DatabaseReference m_databaseRef;


    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    void Start()
    {
        // Check if depen
        dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
        if (dependencyStatus != Firebase.DependencyStatus.Available) {
            Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                    InitFirebase();
                else {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        else
            InitFirebase();
    }

    void InitFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");

        // Setup editor
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");

        m_theTrees = new List<ARTree>();

        // Get all current trees on startup
        FirebaseDatabase.DefaultInstance.GetReference("Trees").GetValueAsync().ContinueWith(SyncCurrentTrees);

        /// Listen for new trees that are added
        /// Is triggered on first child under Trees when app starts?!?!?!?!? Сука Блять
        FirebaseDatabase.DefaultInstance.GetReference("Trees").LimitToLast(1).ChildAdded += (object sender, ChildChangedEventArgs args) =>
        {
            //TODO: use timestamp to prevent first to be added
            Debug.Log("--ChildAdded-- Time: " + args.Snapshot.Child("plantDate").Value.ToString());  
            Debug.Log("--ChildAdded-- "+ args.Snapshot.Child("name").Value.ToString());
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
            }
            if(args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
            {
                Debug.Log("--ChildAdded-- Antal Childs: "+ args.Snapshot.ChildrenCount);
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    string name = childSnapshot.Child("name").Value.ToString();
                    string barkType = childSnapshot.Child("barkType").Value.ToString();
                    string plantDate = childSnapshot.Child("plantDate").Value.ToString();
                    string trackingImage = childSnapshot.Child("trackingImage").Value.ToString();
                    ARTree tree = new ARTree(name, barkType, plantDate, trackingImage);
                    m_theTrees.Add(tree);
                }
            }
        };
        info.text = "Startup completed";
    }

    // Saves trees to Firebase
    public void writeData()
    {
        info.text = "Writing data";


        addNewTree("björk", "dots", "12/19/2940", "poster1");
        addNewTree("tall", "groovy", "13/19/2940", "poster2"); 
        addNewTree("ek", "funkey", "14/19/2940", "poster3");
        info.text = "Data written";
    }

    // Add tree to database
    private void addNewTree(string name, string barkType, string plantDate, string trackingImage)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Trees");

        // Add trees via transaction
        reference.RunTransaction(mutableData =>
        {
            List<object> treeList = mutableData.Value as List<object>;

            if (treeList == null)
                treeList = new List<object>();

            ARTree tree = new ARTree(name, barkType, plantDate, trackingImage);
            treeList.Add(tree.ToDictionary());
            mutableData.Value = treeList;
            return TransactionResult.Success(mutableData);
        });
    }

    private void SyncCurrentTrees(Task<DataSnapshot> task)
    {
        Debug.Log("Find all trees");
        DataSnapshot trees = task.Result;
        Debug.Log("Trees found: " + trees.ChildrenCount);

        foreach(var snapshot in trees.Children)
        {
            Debug.Log(snapshot.Child("name").Value.ToString());
        }
    }
}


