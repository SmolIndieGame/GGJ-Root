using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CameraControl cameraControl;

    public static InputController I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PlayerController.Cancel();
        if (Input.GetKeyDown(KeyCode.Space))
            PlayerController.EndCurrentPlayerTurn();
        cameraControl.Scroll(Input.mouseScrollDelta.y);
    }

    public void OnClick(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider == null)
            return;
        if (!hit.collider.TryGetComponent<Clickable>(out var receiver))
            return;
        receiver.Click();
    }

    public void OnDrag(Vector2 screenPos, bool first)
    {
        if (first)
            cameraControl.Record(screenPos);
        else
            cameraControl.Drag(screenPos);
    }
}
