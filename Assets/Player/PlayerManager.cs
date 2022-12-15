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
                currentSelected.GetComponent<ConnectionScript>().point1 = currentPointHovering;
                //currentSelected.GetComponent<ConnectionScript>().Point2 = mousePos;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed &&
            currentSelected != null)
        {
            if (currentPointHovering != null && currentSelected.tag == "PointConnection" && ! lineDraggingActive)
            {
                Debug.Log("Connecting");
                SetObject(currentSelected);
                //currentSelected.GetComponent<ConnectionScript>().Point2 = mousePos;
                lineDraggingActive = true;
            }
            else
            {
                Debug.Log("No point selected");
                SetObject(currentSelected);
            }
        }
        if (lineDraggingActive && currentSelected.tag == "PointConnection")
        {
            lineDraggingActive = false;
            currentSelected.GetComponent<ConnectionScript>().point2 = currentPointHovering;
        }
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DestroyCurrentSelected();
        }
    }
    public void OnOverlay(InputAction.CallbackContext context)
    {
        if (!settings.activeSelf)
        {
            settings.SetActive(true);
        }
        else
        {
            settings.SetActive(false);
        }
    }
    public void SelectPoint(bool isLocked)
    {
        pointPrefab.GetComponent<PointScript>().isLocked = isLocked;
        SelectPlacableObject(pointPrefab);
    }
    public void SelectLine()
    {
        SelectPlacableObject(linePrefab);
    }
    public void CurrentPointHoveringMouse(GameObject point)
    {
        Debug.Log("Hovering " + point == null ? "null" : point);
        currentPointHovering = point;
    }
    private void SelectPlacableObject(GameObject givenPrefab)
    {
        DestroyCurrentSelected();
        SetObject(givenPrefab);
    }
    private void SetObject(GameObject givenPrefab)
    {
        currentSelected = Instantiate(givenPrefab, mousePos, Quaternion.identity, transform);
        currentSelected.name = currentSelected.tag +"_" + currPointIdx.ToString();
        currPointIdx++;
    }
    private void DestroyCurrentSelected()
    {
        if (currentSelected != null && !currentSelected.IsPrefabInstance())
        {
            Destroy(currentSelected);
            currentSelected = null;
        }
    }
    private Vector2 mousePos
    {
        get
        {
            return cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
