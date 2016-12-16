using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

//namespace Vuforia
//{
public class WorldUIScript : MonoBehaviour, ITrackableEventHandler {

	[Header("Button prefabs")]
	public GameObject growStyleButtonsPrefab;
	public GameObject barkButtonsPrefab;
    public GameObject leafDrawerPrefab;
    public GameObject backToMainPrefab;
    public GameObject buildButtonPrefab;

    public GameObject[] onDrawingDisable;
        
    [Header("Tree")]
    public GameObject editableTree;
    public GameObject displayTrees;

    [Header("Environment")]
    public GameObject platts;
    public GameObject boid;
    public AudioClip clickSound;


    private GameObject activeBoid;
    private GameObject growStyleButtons, barkButtons;
    private GameObject buildButton, backToMainButton;
	private GameObject leafDrawer;
	private int currentMenu = 0;
	private TrackableBehaviour mTrackableBehaviour;
	private bool trackableSeen = false;
    private TreeDatabaseHandler treeDatabase;
    private int currentDisplayed;
    private AudioSource aud;

	// Use this for initialization
	void Start () {
		mTrackableBehaviour = transform.GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
        treeDatabase = GetComponent<TreeDatabaseHandler>();
        aud = GetComponent<AudioSource>();
        editableTree.SetActive(false);

        TreeDatabaseHandler.TreeDestroyed += AdjustIndex;
	}
		
	// Update is called once per frame
	void Update () {
		CheckTouch();
	}

	public void OnTrackableStateChanged(
									TrackableBehaviour.Status previousStatus,
									TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			if(!trackableSeen) {
				GoNext();
				trackableSeen = true;
			}
		}
		else
		{
				
		}
	}

	private void CheckTouch() {
		#if UNITY_ANDROID
		if(Input.touchCount > 0) {
			if (Input.GetTouch(0).phase == TouchPhase.Began) {
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					checkTouchHit(hit);
				}
			}
		}
		#endif

		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			// Construct a ray from the current mouse coordinates
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				checkTouchHit(hit);
			}              
		}
		#endif
	}

	private void checkTouchHit(RaycastHit hit) {
		if(hit.transform.gameObject.tag == "Button") {
            aud.pitch = Random.Range(0.7f, 1f);
            aud.PlayOneShot(clickSound); // Play clicksound

            if(currentMenu == 1) // Start to build has been pressed
            {
                if (hit.transform.gameObject.name == "NextTreeButton")
                    ShowNextTree(false);
                else
                    GoNext();
            }
            else if (currentMenu == 2) // Choosing bark
			{
				hit.transform.GetComponent<ButtonScript>().SetTreeTexture();
				GoNext();
            }
            else if (currentMenu == 3) //
            {
                GoNext();
            }
			else if(currentMenu == 4) //
			{
				hit.transform.GetComponent<ButtonScript>().SetGrowStyle();
				GoNext();
			}
            else if(currentMenu == 5)
            {
                if (hit.transform.gameObject.name == "BackToMainButton")
                    GoNext();
            }
		}
	}

	public void GoNext() {
        Debug.Log("Current menu index: " + currentMenu);
        if (currentMenu == 0) // go from world to build button
        {
            showBuildButton();
            currentMenu++;
        }
		else if (currentMenu == 1) // go from coosing build to chosing bark
		{
            editableTree.SetActive(true); // Show build tree
            HideDispTrees();
            RemoveBuildButton();
			ShowBarkButtons();
			currentMenu++;
		}
		else if (currentMenu == 2) // go from chosing bark to shaping leaves
		{
			RemoveBarkButtons();
            platts.SetActive(false);
			startLeafShaping(); // New leaf is not applied to the tree.
			currentMenu++;
		//	GoNext(); // TOOD: REMOVE THIS and insert leaf shaping
		} 
		else if (currentMenu == 3) // go from shaping leaves to chosing grow style
		{
			endLeafShaping();
			ShowGrowStyleButtons();
            platts.SetActive(true);
			currentMenu++;
		}
		else if (currentMenu == 4) // go from chosing grow style
		{
			RemoveGrowStyleButtons();
            activeBoid = Instantiate(boid, new Vector3(10, 5, 0), Quaternion.identity);
            treeDatabase.SaveTree(); // Save the newly created tree to the database
            ShowBackToMainButton();
            currentMenu++;
		}
		else if (currentMenu == 5) // go from planting tree back to viewing world
		{
            editableTree.SetActive(false); // Hide build tree
            Destroy(activeBoid);
            RemoveBackToMainButton();
            ShowNextTree(true);
			currentMenu = 0;
            GoNext();
		}
	}

	private void ShowGrowStyleButtons() {
		growStyleButtons = Instantiate(growStyleButtonsPrefab);
		growStyleButtons.transform.parent = transform;
	}

	private void ShowBarkButtons() {
		barkButtons = Instantiate(barkButtonsPrefab);
		barkButtons.transform.parent = transform;
	}

    private void showBuildButton()
    {
        buildButton = Instantiate(buildButtonPrefab);
        buildButton.transform.parent = transform;
    }

    private void ShowBackToMainButton()
    {
        backToMainButton = Instantiate(backToMainPrefab);
        backToMainButton.transform.parent = transform;
    }

    private void RemoveGrowStyleButtons() {
		Destroy(growStyleButtons);
	}

	private void RemoveBarkButtons() {
		Destroy(barkButtons);
	}

    private void RemoveBuildButton()
    {
        Destroy(buildButton);
    }

    private void RemoveBackToMainButton()
    {
        Destroy(backToMainButton);
    }

    private void startLeafShaping() {
		leafDrawer = Instantiate(leafDrawerPrefab);
        foreach (GameObject obj in onDrawingDisable) {
            obj.SetActive(false);
        }
    }

    private void endLeafShaping() {
        Destroy(leafDrawer);
        Destroy(GameObject.Find("NextButton(Clone)"));
        foreach (GameObject obj in onDrawingDisable) {
            obj.SetActive(true);
        }
    }

    /// <summary>
    /// Show the next displayTree.
    /// True for show last, false for normal.
    /// </summary>
    public void ShowNextTree(bool showLast)
    {
        if (displayTrees.transform.childCount == 0) return;
        if (showLast)
        {
            currentDisplayed = displayTrees.transform.childCount - 1;
            displayTrees.transform.GetChild(currentDisplayed).gameObject.SetActive(true);
            return;
        }
        if(currentDisplayed >= displayTrees.transform.childCount)
        {
            currentDisplayed = displayTrees.transform.childCount - 1;
        }
        // Hide currently displayed
        displayTrees.transform.GetChild(currentDisplayed).gameObject.SetActive(false);
        // Get next index
        currentDisplayed = (currentDisplayed + 1) % displayTrees.transform.childCount;
        // Show next tree
        displayTrees.transform.GetChild(currentDisplayed).gameObject.SetActive(true);
    }

    /// <summary>
    /// Adjust index if you are on last tre and it gets destroyed
    /// </summary>
    private void AdjustIndex()
    {
        //TODO: make sure "currentDisplayed" index isn't out of bounds.
        // And then show another tree...
    }

    public void HideDispTrees()
    {
        foreach(Transform tree in displayTrees.transform)
        {
            tree.gameObject.SetActive(false);
        }
    }
}
//}