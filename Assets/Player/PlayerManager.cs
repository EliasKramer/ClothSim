using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

enum PlayerState
{
    Selecting,
    Deleting,
    PointSetting
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    public GameObject settings;
    [SerializeField]
    public GameObject cam;
    [SerializeField]
    public GameObject canvas;
    //this object will be instantiated temporarily to show the player where the connection will be placed
    [SerializeField]
    public GameObject lineShowPrefab;
    [SerializeField]
    public GameObject pointPrefab;
    [SerializeField]
    public GameObject linePrefab;
    [SerializeField]
    public float airResistance = 1;
    [SerializeField]
    public float mass = 10;

    private UnityEngine.UI.Slider airResistanceSlider;
    private UnityEngine.UI.Slider massSlider;

    private GameObject lastClickedObjForLine;

    private GameObject currentSelected = null;
    private static int currPointIdx = 0;

    private GameObject pointMouseIsOver = null;

    private bool connectionMode = false;

    private GameObject currenSelectionObj = null;
    private bool mouseDown = false;
    private void Start()
    {
        //find objet with tag AirResistanceSlider in children
        airResistanceSlider = canvas.FindComponentInChildWithTag<UnityEngine.UI.Slider>("SliderAirResistance");
        airResistanceSlider.onValueChanged.AddListener(delegate { airResistance = airResistanceSlider.value; });

        massSlider = canvas.FindComponentInChildWithTag<UnityEngine.UI.Slider>("SliderMass");
        massSlider.onValueChanged.AddListener(delegate { mass = massSlider.value; });
    }
    void Update()
    {
        if (currentSelected != null && gameObjIsPoint(currentSelected))
        {
            currentSelected.transform.position = mousePos;
        }

        if (connectionMode)
        {
            connectionDisplay();
            handleConnectionUpdate();
        }
        else if (!connectionMode && currenSelectionObj != null)
        {
            Destroy(currenSelectionObj);
        }
    }
    private void handleConnectionUpdate()
    {
        if (pointMouseIsOver == null)
        {
            return;
        }
        if (lastClickedObjForLine == pointMouseIsOver)
        {
            Debug.Log("Line could not be set. mouse is over start point");
            return;
        }
        if (lastClickedObjForLine != pointMouseIsOver)
        {
            Debug.Log("set line");
            SetObject(currentSelected);
            currentSelected.GetComponent<ConnectionScript>().point1 = lastClickedObjForLine;
            currentSelected.GetComponent<ConnectionScript>().point2 = pointMouseIsOver;
            lastClickedObjForLine = pointMouseIsOver;
            return;
        }
    }
    private void connectionDisplay()
    {
        if (currenSelectionObj == null)
        {
            currenSelectionObj = Instantiate(lineShowPrefab);
            currenSelectionObj.transform.parent = transform;
        }
        Vector2 point1 = lastClickedObjForLine.transform.position;
        Vector2 point2 = mousePos;
        Vector2 position = (point1 + point2) / 2;
        Vector2 scale = new Vector2(0.1f, (point1 - point2).magnitude);

        currenSelectionObj.transform.position = position;
        currenSelectionObj.transform.localScale = scale;
        currenSelectionObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, point2 - point1);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        mouseDown = context.performed;
        if (currentSelected == null || (settings.activeSelf && canvas.GetComponent<MouseOverHandler>().isMouseOver) || GetComponent<ClothManager>() != null)
        {
            Debug.Log("click could not be connected to an action");
            return;
        }
        if (context.performed)
        {
            if (gameObjIsPoint(currentSelected))
            {
                Debug.Log("set point");
                SetObject(currentSelected);
                return;
            }
            if (gameObjIsLine(currentSelected) && pointMouseIsOver != null)
            {
                lastClickedObjForLine = pointMouseIsOver;
                connectionMode = true;
            }
        }
        if (context.canceled)
        {
            if (gameObjIsLine(currentSelected))
            {
                handleConnectionUpdate();
                connectionMode = false;
                currentSelected = linePrefab;
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("cancel action");

            if (gameObjIsPoint(currentSelected))
            {
                Destroy(currentSelected);
            }
            else if (gameObjIsLine(currentSelected))
            {
                connectionMode = false;
                currentSelected = null;
            }
        }
    }
    public void OnOverlay(InputAction.CallbackContext context)
    {
        if (!settings.activeSelf)
        {
            Debug.Log("overlay true");
            settings.SetActive(true);
        }
        else
        {
            Debug.Log("overlay false");
            settings.SetActive(false);
        }
    }
    public void SelectPoint(bool isLocked)
    {
        Debug.Log("select point - islocked: " + isLocked);
        pointPrefab.GetComponent<PointScript>().isLocked = isLocked;
        SelectPlacableObject(pointPrefab);
    }
    public void SelectLine()
    {
        Debug.Log("select line");
        SelectPlacableObject(linePrefab);
    }
    public void CurrentPointHoveringMouse(GameObject point)
    {
        if (point == null)
        {
            Debug.Log("point is null. setting pointMouseIsOver to null");
            //there was a saved obj before. reset color on that one
            if (pointMouseIsOver != null)
            {
                //pointMouseIsOver.GetComponent<SpriteRenderer>().color = pointMouseIsOver.GetComponent<PointScript>().color;
            }
            pointMouseIsOver = null;
            return;
        }

        if (!gameObjIsPoint(point))
        {
            Debug.Log("trying to set a nonpoint as pointMouseIsOver. returning");
            return;
        }

        Debug.Log("point is not null. setting pointMouseIsOver");
        //point.GetComponent<SpriteRenderer>().color = Color.cyan;
        pointMouseIsOver = point;
    }
    private void SelectPlacableObject(GameObject givenPrefab)
    {
        Debug.Log("select placable object");
        DestroyCurrentSelected();

        if (gameObjIsPoint(givenPrefab))
        {
            SetObject(givenPrefab);
        }
        else if (gameObjIsLine(givenPrefab))
        {
            currentSelected = givenPrefab;
        }
    }
    private void SetObject(GameObject givenPrefab)
    {
        currentSelected = Instantiate(givenPrefab, mousePos, Quaternion.identity, transform);
        currentSelected.name = currentSelected.tag + "_" + currPointIdx.ToString();
        currPointIdx++;
        CurrentPointHoveringMouse(givenPrefab);
        Debug.Log("set current selected object (and instanciante) " + currentSelected.name);
    }
    public void StartSimulating()
    {
        Debug.Log("start simulating");
        GameObject newObj = gameObject.AddComponent<ClothManager>().gameObject;
        newObj.GetComponent<ClothManager>().setMass(mass);
        newObj.GetComponent<ClothManager>().setAirResistance(airResistance);
        settings.SetActive(false);
    }
    public bool isMouseDown()
    {
        return mouseDown;
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void DestroyCurrentSelected()
    {
        Debug.Log("try destroy current selected");
        if (currentSelected == null)
        {
            Debug.Log("current selected is null");
            return;
        }
        Debug.Log("destroy current selected");

        if (!gameObjIsLine(currentSelected))
        {
            Destroy(currentSelected);
        }

        currentSelected = null;
    }
    private bool gameObjIsPoint(GameObject gameObject)
    {
        return gameObject.tag == "Point";
    }
    private bool gameObjIsLine(GameObject gameObject)
    {
        return gameObject.tag == "PointConnection";
    }
    private Vector2 mousePos
    {
        get
        {
            return cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
