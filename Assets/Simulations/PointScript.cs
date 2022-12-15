using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public bool isLocked;

    public Vector2 prevPos;
    private void Start()
    {
        prevPos = transform.position;
    }
    private void FixedUpdate()
    {
        updateColor();
    }
    private void updateColor()
    {
        if (isLocked)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.parent.GetComponent<PlayerManager>().CurrentPointHoveringMouse(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.parent.GetComponent<PlayerManager>().CurrentPointHoveringMouse(null);
    }
}
