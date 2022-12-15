using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PointScript : MonoBehaviour
{
    [SerializeField]
    public bool isLocked;

    public Vector2 prevPos;

    private void Start()
    {
        prevPos = transform.position;
    }
    private void Update()
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
}
