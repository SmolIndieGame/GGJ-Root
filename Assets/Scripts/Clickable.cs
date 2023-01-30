using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    [SerializeField] private GameObject handlerObj;

    IClickHandler handler;

    private void Awake()
    {
        handler = handlerObj.GetComponent<IClickHandler>();
    }

    public void Click() => handler.OnClick();
}
