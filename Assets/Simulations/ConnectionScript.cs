using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionScript : MonoBehaviour
{
    [SerializeField]
    public GameObject point1;
    [SerializeField]
    public GameObject point2;
    [SerializeField]
    public float thickness;

    public float length = 0;
    void Start()
    {
        transform.localScale = new Vector3(thickness, length, 1);
    }

    void Update()
    {
        if(point1 == null || point2 == null)
        {
            return;
        }
        length = ((Vector2)point1.transform.position - (Vector2)point2.transform.position).magnitude;
        transform.localScale = new Vector3(thickness, length, 1);

        transform.position = (point1.transform.position + point2.transform.position) / 2;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, point2.transform.position - point1.transform.position);
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