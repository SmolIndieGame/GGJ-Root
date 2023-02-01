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
    [SerializeField] GameObject becomeTwigParticlePf;
    [SerializeField] GameObject removeTwigParticlePf;

    [SerializeField] SpriteRenderer backGround;

    [SerializeField] GameObject lightPf;

    [SerializeField] Sprite grass;
    [SerializeField] Sprite dirt1;
    [SerializeField] Sprite dirt2;
    [SerializeField] Sprite rock;
    [SerializeField] Sprite water;

    Vector2Int location;
    PlayerNode node;
    BlockType type;
    int childCount;

    float randomValue;

    List<SpriteRenderer> inUseTwigs = new();
    GameObject inUseLight;

    public void Setup(Vector2Int location)
    {
        this.location = location;
        transform.position = (Vector2)location;
        randomValue = Random.value;
        type = (BlockType)(-1);
    }

    public void OnClick()
    {
        PlayerController.Click(location);
    }

    public void UpdateBlockType(BlockType type)
    {
        if (type != this.type)
            backGround.sprite = GetSprite(type);
        this.type = type;
        UpdateTreeStructure(node);
    }

    public void UpdateTreeStructure(PlayerNode node)
    {
        var prevNode = this.node;

        this.node = node;
        foreach (var obj in inUseTwigs)
            VisualTwigPool.I.Release(obj);
        inUseTwigs.Clear();

        if (node == null)
        {
            if (prevNode != null)
            {
                Instantiate(removeTwigParticlePf, (Vector2)location, Quaternion.identity);
                CameraControl.I.Shake(Constants.EatCamShakeIntensity, Constants.EatCamShakeDuration);
                AudioManager.I.Play("Eat");
            }
            Destroy(inUseLight);
            return;
        }

        if (prevNode == null)
        {
            Instantiate(becomeTwigParticlePf, (Vector2)node.Location, Quaternion.identity);
            CameraControl.I.Shake(Constants.MoveCamShakeIntensity, Constants.MoveCamShakeDuration);
            AudioManager.I.Play("Move");
            if (type == BlockType.Water)
                AudioManager.I.Play("Water");

            inUseLight = Instantiate(lightPf, transform.position, Quaternion.identity);
        }

        var parentLoc = node.Location + Vector2Int.up;
        if (node.Parent != null)
            parentLoc = node.Parent.Location;

        GenerateTwigAtDirection(parentLoc - node.Location);

        using var iter = node.GetChildrenEnumerator();
        while (iter.MoveNext())
            GenerateTwigAtDirection(iter.Current.Location - node.Location);
        
        if (childCount > node.ChildrenCount)
            Instantiate(removeTwigParticlePf, (Vector2)location, Quaternion.identity);
        childCount = node.ChildrenCount;
    }

    private void GenerateTwigAtDirection(Vector2Int dir)
    {
        SpriteRenderer obj;
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) == 1)
        {
            obj = VisualTwigPool.I.Get(transform, TwigType.TopOnly);
            obj.transform.up = (Vector2)dir;
            //if (obj.transform.eulerAngles.z == 180)
            //    obj.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            obj = VisualTwigPool.I.Get(transform, TwigType.TopLeftOnly);
            obj.transform.up = (Vector2)dir;
            obj.transform.Rotate(0, 0, -45);
        }

        inUseTwigs.Add(obj);
    }

    Sprite GetSprite(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Dirt => GetDirtTexture(),
            BlockType.Rock => rock,
            BlockType.Water => water,
            _ => GetDirtTexture()
        };
    }

    Sprite GetDirtTexture()
    {
        if (location.y == 0)
            return grass;
        return randomValue < 0.5f ? dirt1 : dirt2;
    }
}
