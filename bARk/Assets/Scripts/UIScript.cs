using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    public Button Main;
    public Button SetColor;

    public Material colorTest;

    private Color color;
    private float r = 0.8f , g = 0.1f, b = 0.1f;

    private int currentMenu = 0;

    Slider colorslide;
    GameObject colorslideObject;

    [Header("Leaf Settings")]
    public Vector3 leafFinalPos;
    public float leafScale = 0.2f;
    public GameObject leafDrawerPrefab;


    // Use this for initialization
    void Start () {
        colorTest.color = new Color(r, g, b, 1.0f);
        SetColor.GetComponent<Button>().gameObject.SetActive(false);
        colorslide = SetColor.GetComponentInChildren<Slider>();
        colorslideObject = colorslide.gameObject;
    }

    public void changeColorSlider(float test)
    {
        float frequence = 0.3f;
        r = (Mathf.Sin(frequence * test + 0)* 127 + 128)/255;
        g = (Mathf.Sin(frequence * test + 2) * 127 + 128)/255;
        b = (Mathf.Sin(frequence * test + 4) * 127 + 128)/255;
        colorTest.color = new Color(r, g, b, 1.0f);
    }

    public void changeColor() //show/hide slider
    {
        if (!colorslideObject.activeSelf)
        {
            colorslideObject.SetActive(true);
        }
        else
        {
            colorslideObject.SetActive(false);
        }        
    }

    public void GoNext()
    {
        if (currentMenu == 0) //go from viewing world to shaping tree
        {
            SetColor.GetComponent<Button>().gameObject.SetActive(true);
            colorslideObject.SetActive(false);
            Main.GetComponentInChildren<Text>().text = "Shape leaves! --->";
            currentMenu++;
        }
        else if (currentMenu == 1) //go from shaping tree to shaping leaves
        {
            SetColor.GetComponent<Button>().gameObject.SetActive(false);
            Main.GetComponentInChildren<Text>().text = "Preview tree! --->";
            startLeafShaping();
            currentMenu++;
        } 
        else if (currentMenu == 2) //go from shaping leaves to previewing tree
        {
            endLeafShaping();
            Main.GetComponentInChildren<Text>().text = "Plant your tree! --->";
            currentMenu++;
        }
        else if (currentMenu == 3) //go from previewing tree to planting tree
        {
            Main.GetComponentInChildren<Text>().text = "View your tree! --->";
            currentMenu++;
        }
        else if (currentMenu == 4) //go from planting tree back to viewing world
        {
            Main.GetComponentInChildren<Text>().text = "Make Tree! --->";
            currentMenu = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (currentMenu == 0) //use camera to show trees
        {
            //code for showing world + trees
        }
        else if (currentMenu == 1) //build your tree
        { 
            //code for building a tree
        }
        else if (currentMenu == 2) //shape your leaves
        {
            //code for shaping leaves
        }
        else if (currentMenu == 3) //preview tree
        {
            //code for viewing tree with leaves on
        }
        else if (currentMenu == 4)  //planting tree
        {
            //code for planting tree (animation?)
        }
	}

    private void startLeafShaping() {
        Instantiate(leafDrawerPrefab);
        Vector3 camPos = Camera.main.transform.position;
        camPos.z -= 20;
        Camera.main.transform.position = camPos;
    }

    private void endLeafShaping() {
        Vector3 camPos = Camera.main.transform.position;
        camPos.z += 20;
        Camera.main.transform.position = camPos;
        GameObject leaf = GameObject.Find("Leaf(Clone)");
        leaf.transform.position = leafFinalPos;
        leaf.transform.localScale = new Vector3(leafScale, leafScale, 0.01f);
    }

}
