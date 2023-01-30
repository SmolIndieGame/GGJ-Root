using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float speed;

    void Update()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        float dy = Input.GetAxisRaw("Vertical");
        if (Mathf.Approximately(dx, 0) && Mathf.Approximately(dy, 0))
            return;
        transform.position += speed * Time.deltaTime * new Vector3(dx, dy);
    }
}
