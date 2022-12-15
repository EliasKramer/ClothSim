using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        Debug.Log("Mouse entered UI");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        Debug.Log("Mouse exited UI");
    }
}
