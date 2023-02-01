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
    [SerializeField] Sprite topOnly;
    [SerializeField] Sprite topLeftOnly;
    [SerializeField] Sprite topToButtom;
    [SerializeField] Sprite topToLeft;
    [SerializeField] Sprite topToLeftButtom;
    [SerializeField] Color randomColor;

    private void Awake()
    {
        I = this;
    }

    public SpriteRenderer Get(Transform parent, TwigType twigType)
    {
        if (!pool.TryPop(out var obj))
            obj = Instantiate(twigPf).GetComponent<SpriteRenderer>();
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.sprite = twigType switch
        {
            TwigType.TopOnly => topOnly,
            TwigType.TopLeftOnly => topLeftOnly,
            TwigType.TopToButtom => topToButtom,
            TwigType.TopToRight => topToLeft,
            TwigType.TopToRightButtom => topToLeftButtom,
            _ => null
        };
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(SpriteRenderer twigObj)
    {
        twigObj.gameObject.SetActive(false);
        pool.Push(twigObj);
    }
}
