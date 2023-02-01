using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl I { get; private set; }

    [SerializeField] Camera cam;
    [SerializeField] Transform shaker;

    private Vector2 dragOriginPos;

    float currentShakeIntensity;
    float currentShakeDuration;

    private void Awake()
    {
        I = this;
    }

    public void Record(Vector2 screenPos)
    {
        dragOriginPos = cam.ScreenToWorldPoint(screenPos);
    }

    public void Drag(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        transform.position -= (Vector3)(worldPos - dragOriginPos);
    }

    public void Scroll(float delta)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta, 2.5f, 10);
    }

    public void Shake(float intensity, float duration)
    {
        if (currentShakeIntensity > intensity)
            return;
        currentShakeIntensity = intensity;
        currentShakeDuration = duration;
    }

    private void Update()
    {
        if (currentShakeIntensity < 0.01f)
        {
            currentShakeIntensity = 0;
            return;
        }

        shaker.localPosition = Random.insideUnitCircle * currentShakeIntensity;
        currentShakeIntensity *= currentShakeDuration;
    }
}
