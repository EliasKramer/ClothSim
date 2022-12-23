using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public bool isLocked;

    public Vector2 prevPos;
    public Color color;
    private void Start()
    {
        prevPos = transform.position;
        updateColor();
    }
    private void updateColor()
    {
        color = isLocked ? Color.red : Color.white;
        GetComponent<SpriteRenderer>().color = color;
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
