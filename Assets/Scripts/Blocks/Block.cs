using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IClickHandler
{
    [SerializeField] new SpriteRenderer renderer;

    [SerializeField] Color clickedColor;
    [SerializeField] Color usedColor;

    Vector2Int location;

    public void Setup(Color color, Vector2Int location)
    {
        renderer.color = color;
        this.location = location;
        transform.position = (Vector2)location;
    }

    public void OnClick()
    {
        BlockGrid.Click(location);
    }

    public void SetToCurrent() => renderer.color = clickedColor;
    public void SetToUsed() => renderer.color = usedColor;
}
