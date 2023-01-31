using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector2 dragOriginPos;
    private Vector2 startPos;

    public void Record(Vector2 screenPos)
    {
        dragOriginPos = cam.ScreenToWorldPoint(screenPos);
        startPos = transform.position;
    }

    public void Drag(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        transform.position -= (Vector3)(worldPos - dragOriginPos);
    }

    public void Scroll(float delta)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta, 4, 10);
    }
}
