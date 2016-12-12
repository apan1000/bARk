using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class DatabaseHandler2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bark-11fd5.firebaseio.com/");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
