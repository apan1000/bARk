using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vuforia
{
	public class WorldUIScript : MonoBehaviour, ITrackableEventHandler {

		[Header("Button prefabs")]
		public GameObject growStyleButtonsPrefab;
		public GameObject barkButtonsPrefab;

		private GameObject growStyleButtons, barkButtons;
		private int currentMenu = 0;
		private TrackableBehaviour mTrackableBehaviour;
		private bool trackableSeen = false;

		// Use this for initialization
		void Start () {
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (mTrackableBehaviour)
			{
				mTrackableBehaviour.RegisterTrackableEventHandler(this);
			}
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
						if(hit.transform.gameObject.tag == "Button") {
							if(currentMenu == 1) // Choosing bark
							{
								hit.transform.GetComponent<ButtonScript>().SetTreeTexture();
								GoNext();
							}
							else if(true) //
							{

							}
							
						}
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
					if(hit.transform.gameObject.tag == "Button") {
						if(currentMenu == 1) // Choosing bark
						{
							hit.transform.GetComponent<ButtonScript>().SetTreeTexture();
							GoNext();
						}
						else if(true) //
						{

						}
						
					}
				}              
			}
			#endif
		}

		public void GoNext() {
			if (currentMenu == 0) // go from viewing world to chosing bark
			{
				ShowBarkButtons();
				currentMenu++;
			}
			else if (currentMenu == 1) // go from shaping tree to shaping leaves
			{
				// startLeafShaping();
				currentMenu++;
			} 
			else if (currentMenu == 2) // go from shaping leaves to 
			{
				// endLeafShaping();
				currentMenu++;
			}
			else if (currentMenu == 3) //
			{
				currentMenu++;
			}
			else if (currentMenu == 4) // go from planting tree back to viewing world
			{
				currentMenu = 0;
			}
		}

		private void ShowGrowStyleButtons() {
			growStyleButtons = Instantiate(growStyleButtonsPrefab);
		}

		private void ShowBarkButtons() {
			growStyleButtons = Instantiate(barkButtonsPrefab);
		}
	}
}