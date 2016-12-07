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
    string m_name;
    string m_barkType;
    string m_plantDate;
    string m_trackingImage;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    void Start()
    {
        // Extra safety stuff
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

        // Get all current trees
        // FirebaseDatabase.DefaultInstance.GetReference("Trees").GetValueAsync().ContinueWith(SyncCurrentTrees);


       /// Listen for new trees that are added
       /// Finds all current trees when app is started??
        FirebaseDatabase.DefaultInstance.GetReference("Trees").ChildAdded += (object sender, ChildChangedEventArgs args) =>
        {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
            }
            if(args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
            {
                Debug.Log("Antal Childs: "+ args.Snapshot.ChildrenCount);
                foreach(var childSnapshot in args.Snapshot.Children)
                {
                    Debug.Log("Alla childs");
                    Debug.Log(childSnapshot.Child("name").Value.ToString());
                    string name = childSnapshot.Child("name").Value.ToString();
                    string barkType = childSnapshot.Child("barkType").Value.ToString();
                    string plantDate = childSnapshot.Child("plantDate").Value.ToString();
                    string trackingImage = childSnapshot.Child("trackingImage").Value.ToString();
                    ARTree tree = new ARTree(name, barkType, plantDate, trackingImage);
                    m_theTrees.Add(tree);
                    UpdateText();
                }
            }
        };
        info.text = "Startup completed";
    }

    private void UpdateText()
    {
        Debug.Log(m_theTrees.Count.ToString());
    }

    private void TreeAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            info.text = args.DatabaseError.Message;
            return;
        }
        info.text = "new Tree added!";
        args.Snapshot.Child("trees");
    }

    public void writeData()
    {
        info.text = "Writing data";
        addNewTree("björk", "dots", "12/19/2940", "poster1");
        addNewTree("tall", "groovy", "13/19/2940", "poster2");
        addNewTree("ek", "funkey", "14/19/2940", "poster3");
        info.text = "Data written";
    }

    // Add trees to Firebase Via transactions
    private void addNewTree(string name, string barkType, string plantDate, string trackingImage)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Trees");

        m_name = name;
        m_barkType = barkType;
        m_plantDate = plantDate;
        m_trackingImage = trackingImage;

        reference.RunTransaction(AddScoreTransaction).ContinueWith(task => {
            if (task.Exception != null)
            {
                Debug.Log(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Transaction complete.");
            }
        });
    }
    public TransactionResult AddScoreTransaction(MutableData mutableData)
    {
        List<object> treeList = mutableData.Value as List<object>;

        if(treeList == null) {
            treeList = new List<object>();
        }
        ARTree tree = new ARTree(m_name, m_barkType, m_plantDate, m_trackingImage);
        treeList.Add(tree.ToDictionary());
        mutableData.Value = treeList;
        return TransactionResult.Success(mutableData);
    }

    // This only runs when the tree node is empty on Firebase
    //private void SyncCurrentTrees(Task<DataSnapshot> task)
    //{
    //    Debug.Log("Hallåpp!!");
    //    if (task.IsFaulted) {
    //        Debug.Log("FAILED TO SYNC");
    //    }
    //    else if (task.IsCompleted){
    //        DataSnapshot snapshot = task.Result;
    //        foreach (var childSnapshot in snapshot.Children)
    //        {
    //            Debug.Log("Sync on Startup!");
    //            string name = childSnapshot.Child("name").Value.ToString();
    //            string barkType = childSnapshot.Child("barkType").Value.ToString();
    //            string plantDate = childSnapshot.Child("plantDate").Value.ToString();
    //            string trackingImage = childSnapshot.Child("trackingImage").Value.ToString();
    //            ARTree tree = new ARTree(name, barkType, plantDate, trackingImage);
    //            m_theTrees.Add(tree);
    //            UpdateText();
    //        }
    //    }
    //}
}

// Other classes

public class ARTree
{
    private string name;
    private string barkType;
    private string plantDate;
    private string trackingImage;
    private string ownerID;

    public ARTree(string name, string barkType, string plantDate, string trackingImage)
    {
        this.name = name;
        this.barkType = barkType;
        this.plantDate = plantDate;
        this.trackingImage = trackingImage;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["ownerID"] = ownerID;
        result["name"] = name;
        result["barkType"] = barkType;
        result["plantDate"] = plantDate;
        result["trackingImage"] = trackingImage;
        result["trackingImage2"] = trackingImage;
        result["trackingImage3"] = trackingImage;
        return result;
    }
}

public class User
{
    public string username;
    public string email;

    public User(string username, string email)
    {
        this.username = username;
        this.email = email;
    }
}