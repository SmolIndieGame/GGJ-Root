using System.Collections.Generic;
using UnityEngine;

public class BlockInfo
{
    public Vector2Int location;
    public BlockType blockType;
    public Block block;
}

public enum BlockType
{
    Dirt,
    Rock,
    Water,
    Root
}

public static class BlockGrid
{
    static readonly Dictionary<Vector2Int, BlockInfo> blocks;
    static BlockInfo currentBlock;

    // TODO: please change
    public static int score { get; private set; }

    static BlockGrid()
    {
        score = 10;
        blocks = new();
        BlockGenerator.I.GenerateBlock(Vector2Int.zero);
        SetCurrent(blocks[Vector2Int.zero]);
        GenerateAround(Vector2Int.zero);
    }

    public static void Init() { }

    public static void Add(Vector2Int location, BlockInfo info)
    {
        blocks.Add(location, info);
    }

    public static bool Contains(Vector2Int location) => blocks.ContainsKey(location);

    public static bool TryGet(Vector2Int location, out BlockInfo info)
    {
        return blocks.TryGetValue(location, out info);
    }

    public static void Click(Vector2Int location)
    {
        var delta = location - currentBlock.location;
        if (delta.y > 0 || Mathf.Abs(delta.x) + Mathf.Abs(delta.y) != 1)
            return;
        var target = blocks[location];
        switch (target.blockType)
        {
            case BlockType.Dirt:
                score -= 1;
                break;
            case BlockType.Rock:
                score -= 3;
                break;
            case BlockType.Water:
                score += 6;
                break;
            case BlockType.Root:
            default:
                return;
        }
        GenerateAround(location);
        SetCurrent(target);
    }

    static void SetCurrent(BlockInfo info)
    {
        currentBlock?.block.SetToUsed();
        info.blockType = BlockType.Root;
        info.block.SetToCurrent();
        currentBlock = info;
    }

    static void GenerateAround(Vector2Int location)
    {
        BlockGenerator.I.GenerateBlock(location + Vector2Int.down);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.left);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.right);

        BlockGenerator.I.GenerateBlock(location + Vector2Int.down * 2);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.left * 2);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.right * 2);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.down + Vector2Int.right);
        BlockGenerator.I.GenerateBlock(location + Vector2Int.down + Vector2Int.left);
    }
}
