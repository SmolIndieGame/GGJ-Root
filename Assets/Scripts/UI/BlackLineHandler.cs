using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackLineHandler : MonoBehaviour
{
    [SerializeField] BlockGenerator generator;

    private void Update()
    {
        transform.position = new Vector3(-0.5f, -generator.CurrentHeight + 0.5f);
    }
}
