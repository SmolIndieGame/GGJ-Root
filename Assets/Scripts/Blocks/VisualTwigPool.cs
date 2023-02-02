using System.Collections.Generic;
using UnityEngine;

public enum TwigType
{
    TopOnly,
    TopLeftOnly,
    TopToButtom,
    TopToRight,
    TopToRightButtom
}

public class VisualTwigPool : MonoBehaviour
{
    public static VisualTwigPool I { get; private set; }

    readonly Stack<SpriteRenderer> pool = new();

    [SerializeField] private GameObject twigPf;

    private void Awake()
    {
        I = this;
    }

    public SpriteRenderer Get(Transform parent)
    {
        if (!pool.TryPop(out var obj))
            obj = Instantiate(twigPf).GetComponent<SpriteRenderer>();
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(SpriteRenderer twigObj)
    {
        twigObj.gameObject.SetActive(false);
        pool.Push(twigObj);
    }
}
