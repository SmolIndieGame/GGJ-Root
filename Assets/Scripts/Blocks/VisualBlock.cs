using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AllianceColor
{
    public Color twig;
    public Color leaf;
}

public class VisualBlock : MonoBehaviour, IClickHandler
{
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] Color clickedColor;
    [SerializeField] Color usedColor;

    Vector2Int location;
    AllianceColor alliance;

    public void Setup(Vector2Int location)
    {
        this.location = location;
        transform.position = (Vector2)location;
    }

    public void OnClick()
    {
        PlayerController.Click(location);
    }

    public void UpdateBlockType(BlockType type)
    {
        spriteRenderer.color = GetColor(type);
    }

    public void ChangeAlliance(AllianceColor alliance)
    {
        this.alliance = alliance;
    }

    Color GetColor(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Dirt => Color.white,
            BlockType.Rock => Color.gray,
            BlockType.Water => Color.cyan,
            BlockType.Twig => alliance.twig,
            BlockType.Leaf => alliance.leaf,
            BlockType.DeadTwig => new Color32(50, 50, 0, 255),
            _ => Color.white,
        };
    }
}
