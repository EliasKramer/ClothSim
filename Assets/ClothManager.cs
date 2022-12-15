using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class ClothManager : MonoBehaviour
{
    [SerializeField]
    public GameObject clothPointPrefab;
    [SerializeField]
    public GameObject clothLinePrefab;
    [SerializeField]
    public GameObject cam;

    private float distanceBetweenPoints = 1f;
    private float pointRadius;
    private Vector2 startPoint;
    private Vector2 camSize;
    
    [SerializeField]
    public float airResistance = 1;
    [SerializeField]
    public float mass = 10;

    private float iterationsForStability = 100;

    private List<GameObject> points = new List<GameObject>();
    private List<GameObject> connections = new List<GameObject>();

    void Start()
    {
        pointRadius = clothPointPrefab.transform.localScale.x / 2;
        Vector2 camPixelRect = cam.GetComponent<Camera>().pixelRect.size;
        camSize = cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(camPixelRect.x, camPixelRect.y));

        //find all points and lines in scene
        Debug.Log("added:");
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("Point"))
        {
            points.Add(point);
            Debug.Log(point.name);
        }
        foreach (GameObject line in GameObject.FindGameObjectsWithTag("PointConnection"))
        {
            connections.Add(line);
            Debug.Log(line.name);
        }
    }
    void Update()
    {
        foreach (GameObject point in points)
        {
            PointScript pointScript = point.GetComponent<PointScript>();
            if (!pointScript.isLocked)
            {
                Vector2 posBefore = point.transform.position;
                float deltaTime = Mathf.Min(.008f, Time.deltaTime);
                point.transform.position += (Vector3)((Vector2)point.transform.position - pointScript.prevPos) * (1 - (deltaTime * airResistance));
                point.transform.position += (Vector3)(Vector2.down * mass * deltaTime * deltaTime);
                pointScript.prevPos = posBefore;
            }
        }
        for (int i = 0; i < iterationsForStability; i++)
        {
            foreach (GameObject connection in connections)
            {
                ConnectionScript connectionScript = connection.GetComponent<ConnectionScript>();

                Vector2 connectionCenter = (connectionScript.Point1 + connectionScript.Point2) / 2;
                Vector2 connectionDir = (connectionScript.Point1 - connectionScript.Point2).normalized;
                if (!connectionScript.point1.GetComponent<PointScript>().isLocked)
                {
                    connectionScript.Point1 = connectionCenter + (connectionDir * connectionScript.Length / 2);
                }
                if (!connectionScript.point2.GetComponent<PointScript>().isLocked)
                {
                    connectionScript.Point2 = connectionCenter - (connectionDir * (connectionScript.Length / 2));
                }
            }
        }
    }
}
