using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuider : MonoBehaviour
{
    [SerializeField] GameObject guideBlockPf;

    readonly HashSet<GuideBlock> inUse = new();

    readonly Stack<GuideBlock> pool = new();

    private void Awake()
    {
        PlayerController.visualGuider = this;
    }

    public void PlaceTargetAt(Vector2Int loc)
    {
        if (!pool.TryPop(out GuideBlock obj))
            obj = Instantiate(guideBlockPf).GetComponent<GuideBlock>();
        obj.gameObject.SetActive(true);
        obj.Setup(loc, 0, true);
        inUse.Add(obj);
    }

    public void PlaceGuideAt(Vector2Int loc, float score)
    {
        if (!pool.TryPop(out GuideBlock obj))
            obj = Instantiate(guideBlockPf).GetComponent<GuideBlock>();
        obj.gameObject.SetActive(true);
        obj.Setup(loc, score, false);
        inUse.Add(obj);
    }

    public void RemoveAllGuide()
    {
        foreach (var block in inUse)
        {
            block.gameObject.SetActive(false);
            pool.Push(block);
        }
        inUse.Clear();
    }
}
