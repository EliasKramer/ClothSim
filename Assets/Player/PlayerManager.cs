using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    public GameObject settings;
    [SerializeField]
    public GameObject cam;
    [SerializeField]
    public GameObject canvas;

    public GameObject pointPrefab;
    public GameObject linePrefab;

    private GameObject currentSelected = null;
    private static int currPointIdx = 0;
    private bool lineDraggingActive = false;

    private GameObject currentPointHovering = null;

    // Update is called once per frame
    void Update()
    {
        if (currentSelected != null)
        {
            currentSelected.transform.position = mousePos;
            if (lineDraggingActive)
            {
                //currentSelected.GetComponent<ConnectionScript>().point1 = currentPointHovering;
                //currentSelected.GetComponent<ConnectionScript>().Point2 = mousePos;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed || currentSelected == null || canvas.GetComponent<MouseOverHandler>().isMouseOver)
        {
            Debug.Log("click could not be connected to an action");
            return;
        }

        if(currentSelected.tag == "Point")
        {
            if (currentPointHovering == null)
            {
                Debug.Log("Point could not be set. there was another one in the way");
                return;
            }
            Debug.Log("set point");
            SetObject(currentSelected);
            return;
        }
        
        if(currentPointHovering != null)
        {
            if (lineDraggingActive && currentSelected.GetComponent<ConnectionScript>().point1 == currentPointHovering)
            {
                Debug.Log("the start point was the end point. could not be set");
                return;
            }
            if (lineDraggingActive)
            {
                Debug.Log("set connector end");
                currentSelected.GetComponent<ConnectionScript>().point2 = currentPointHovering;
                SetObject(currentSelected);
                lineDraggingActive = false;
            }
            else
            {
                Debug.Log("set connector start");
                currentSelected.GetComponent<ConnectionScript>().point1 = currentPointHovering;
                //SetObject(currentSelected);
                lineDraggingActive = true;
            }
        }
        else
        {
            Debug.Log("Line could not be set. no point to start or finish");
            return;
        }
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("cancel action");
            DestroyCurrentSelected();
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
        Debug.Log("Hovering " + (point == null ? "null" : point));
        currentPointHovering = point;
    }
    private void SelectPlacableObject(GameObject givenPrefab)
    {
        Debug.Log("select placable object");
        DestroyCurrentSelected();
        SetObject(givenPrefab);
    }
    private void SetObject(GameObject givenPrefab)
    {
        currentSelected = Instantiate(givenPrefab, mousePos, Quaternion.identity, transform);
        currentSelected.name = currentSelected.tag + "_" + currPointIdx.ToString();
        currPointIdx++;
        Debug.Log("set current selected object (and instanciante) " + currentSelected.name);
    }
    private void DestroyCurrentSelected()
    {
        Debug.Log("try destroy current selected");
        if (currentSelected == null)
        {
            Debug.Log("current selected is null");
            return;
        }
        if (currentSelected.IsPrefabInstance())
        {
            Debug.Log("cant destroy prefab");
            Destroy(currentSelected);
        }
        Debug.Log("destroy current selected");
        Destroy(currentSelected);
        currentSelected = null;
        lineDraggingActive = false;
    }
    private Vector2 mousePos
    {
        get
        {
            return cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
