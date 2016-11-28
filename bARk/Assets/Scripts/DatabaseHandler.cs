using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

// Firebase can only be accessed when running the application on a phone
// Error will occur if you try to access it in play mode
public class DatabaseHandler : MonoBehaviour
{
    public Text info;
    public Button[] buttons;

    private DatabaseReference databaseRef;

    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;

    private string email = "staffan.sandberg94@gmail.com";
    private string password = "abccbe123321";

    void Start()
    {
        // Get root reference location of database
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Default instance from authenticator
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        // Setup editor
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");
        info.text = "Startup completed";

        var reference = FirebaseDatabase.DefaultInstance.GetReference("trees");
        reference.ChildAdded += (object sender, ChildChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                info.text = args.DatabaseError.Message;
                return;
            }
            info.text = "new Tree added!";
            args.Snapshot.Child("trees");
        };
    }

    public void writeData()
    {
        writeNewUser(user.UserId, "Staffan", email);
        addNewTree(user.UserId, "björk", "dots", "12/19/2940", "poster1");
        addNewTree(user.UserId, "tall", "groovy", "13/19/2940", "poster2");
        addNewTree(user.UserId, "ek", "funkey", "14/19/2940", "poster3");
        info.text = "Data written";
    }

    private void addNewTree(string userID, string name, string barkType, string plantDate, string trackingImage)
    {
        string key = databaseRef.Child("trees").Push().Key;
        ARTree tree = new ARTree(user.UserId, name, barkType, plantDate, trackingImage);
        Dictionary<string, object> entryVal = tree.ToDictionary();

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/trees/" + key] = entryVal;

        databaseRef.UpdateChildrenAsync(childUpdates);
    }

    private void writeNewUser(string userID, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        // Save new user under users and userID
        databaseRef.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }

    // Log in with email and password
    public void LoginUserWithEmail()
    {
        DisableUI();
        info.text = "LOGIN START";
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(SignInHandler);
    }

    void SignInHandler(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        if (authTask.IsCanceled)
            info.text = "Task was canceled.";
        else if (authTask.IsFaulted)
            info.text = "Task faulted.";
        else if (authTask.IsCompleted)
        {
            info.text = "Sign in completed.";
            if(auth.CurrentUser != null)
            {
                info.text = "Current user:\n" + auth.CurrentUser.Email;
            }
        }
        EnableUI();
    }

    public void CreateUserWithEmail()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    // User created
                    info.text = "User created";
                }
                else
                {
                    // User creation failed
                    info.text = "User creation failed";
                }
                if (task.IsCanceled) { info.text = "Task canceled!!"; }
                if (task.IsFaulted) { info.text = "Task faulted!!"; }
            });
    }


    void DisableUI()
    {
        foreach (Button b in buttons)
            b.interactable = false;
    }
    void EnableUI()
    {
        foreach (Button b in buttons)
            b.interactable = true;
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

public class ARTree
{
    private string name;
    private string barkType;
    private string plantDate;
    private string trackingImage;
    private string ownerID;

    public ARTree(string ownerID, string name, string barkType, string plantDate, string trackingImage)
    {
        this.ownerID = ownerID;
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

        return result;
    }
}