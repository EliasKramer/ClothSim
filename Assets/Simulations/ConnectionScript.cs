using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionScript : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public float thickness;

    public float length = 0;
    void Start()
    {
        Physics.queriesHitTriggers = true;
        transform.localScale = new Vector3(thickness, length, 1);
    }

    void Update()
    {
        if (point1 == null || point2 == null)
        {
            return;
        }
        length = ((Vector2)point1.transform.position - (Vector2)point2.transform.position).magnitude;
        transform.localScale = new Vector3(thickness, length, 1);

        transform.position = (point1.transform.position + point2.transform.position) / 2;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, point2.transform.position - point1.transform.position);
    }
    public void OnMouseOver()
    {
        if (transform.parent.GetComponent<ClothManager>() != null && transform.parent.GetComponent<PlayerManager>().isMouseDown())
        {
            transform.parent.GetComponent<ClothManager>().removeConnection(gameObject);
            Destroy(this.gameObject);
        }
    }

    public Vector2 Point1
    {
        set
        {
            point1.transform.position = value;
        }
        get
        {
            return point1.transform.position;
        }
    }
    public Vector2 Point2
    {
        set
        {
            point2.transform.position = value;
        }
        get
        {
            return point2.transform.position;
        }
    }
    public float Length
    {
        get
        {
            return length;
        }
    }
}