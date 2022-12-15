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

    private float length;
    void Start()
    {
        length = ((Vector2)point1.transform.position - (Vector2)point2.transform.position).magnitude;
        transform.localScale = new Vector3(thickness, length, 1);
    }

    void Update()
    {
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