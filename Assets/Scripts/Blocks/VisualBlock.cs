using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerTextureSet
{
    public Sprite top;
    public Sprite topLeft;
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

        GenerateTwigAtDirection(parentLoc - node.Location, true);

        using var iter = node.GetChildrenEnumerator();
        while (iter.MoveNext())
            GenerateTwigAtDirection(iter.Current.Location - node.Location, false);
        
        if (childCount > node.ChildrenCount)
            Instantiate(removeTwigParticlePf, (Vector2)location, Quaternion.identity);
        childCount = node.ChildrenCount;
    }

    private void GenerateTwigAtDirection(Vector2Int dir, bool isParent)
    {
        SpriteRenderer obj;
        int dis = Mathf.Abs(dir.x) + Mathf.Abs(dir.y);

        obj = VisualTwigPool.I.Get(transform);
        obj.sprite = dis == 1 ? node.Tree.alliance.top : node.Tree.alliance.topLeft;
        obj.transform.up = (Vector2)(isParent ? dir : -dir);
        if (!isParent)
        {
            obj.transform.position -= 0.25f * dis * obj.transform.up;
            obj.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (dis == 2)
            obj.transform.Rotate(0, 0, isParent ? -45 : 45);

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
