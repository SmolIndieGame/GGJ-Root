using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Update()
    {
        if (!Input.GetButtonDown("Fire1"))
            return;
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider == null)
            return;
        if (!hit.collider.TryGetComponent<Clickable>(out var receiver))
            return;
        receiver.Click();
    }
}
