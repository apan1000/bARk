using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIScript : MonoBehaviour {

	[Header("Button prefabs")]
	public GameObject growStyleButtonsPrefab;
	public GameObject barkButtonsPrefab;

	private GameObject growStyleButtons, barkButtons;
	private int currentMenu = 0;

	// Use this for initialization
	void Start () {
		ShowGrowStyleButtons();
	}
	
	// Update is called once per frame
	void Update () {
		CheckTouch();
	}

	private void CheckTouch() {
		#if UNITY_ANDROID
		// for (int i = 0; i < Input.touchCount; ++i) {
		if(Input.touchCount > 0) {
			if (Input.GetTouch(0).phase == TouchPhase.Began) {
				
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					if(hit.transform.gameObject.tag == "Button") {
						if(currentMenu == 1) //shaping tree
						{
							hit.transform.GetComponent<ButtonScript>().SetTreeTexture();
							// ButtonScript bs = (ButtonScript) hit.transform.gameObject.GetComponent(typeof(ButtonScript));
							// bs.setTreeTexture();
							GoNext();
						}
						else if(true) //Choosing bark
						{

						}
						
					}
				}
			}
		}
		#endif

		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			print("Touch!");
			
			// Construct a ray from the current touch coordinates
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				if(hit.transform.gameObject.tag == "Button") {
					print("Button clicked!");
					if(currentMenu == 1) //shaping tree
					{
						hit.transform.GetComponent<ButtonScript>().SetTreeTexture();
						// ButtonScript bs = (ButtonScript) hit.transform.gameObject.GetComponent(typeof(ButtonScript));
						// bs.setTreeTexture();
						GoNext();
					}
					else if(true) //Choosing bark
					{

					}
					
				}
			}              
		}
		#endif
	}

	public void GoNext() {
        if (currentMenu == 0) //go from viewing world to shaping tree
        {
            currentMenu++;
        }
        else if (currentMenu == 1) //go from shaping tree to shaping leaves
        {
            // startLeafShaping();
            currentMenu++;
        } 
        else if (currentMenu == 2) //go from shaping leaves to chosing bark
        {
            // endLeafShaping();
            currentMenu++;
        }
        else if (currentMenu == 3) //
        {
            currentMenu++;
        }
        else if (currentMenu == 4) //go from planting tree back to viewing world
        {
            currentMenu = 0;
        }
    }

	private void ShowGrowStyleButtons() {
		growStyleButtons = Instantiate(barkButtonsPrefab);
	}
}
