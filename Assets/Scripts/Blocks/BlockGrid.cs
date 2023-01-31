﻿using System.Collections.Generic;
using UnityEngine;

public class BlockInfo
{
    public readonly Vector2Int location;
    readonly VisualBlock block;
    BlockType blockType;
    PlayerNode treeNode;

    public BlockInfo(Vector2Int location, BlockType blockType, VisualBlock block)
    {
        this.location = location;
        this.blockType = blockType;
        this.block = block;
        treeNode = null;

        block.UpdateBlockType(blockType);
    }

    public BlockType BlockType
    {
        get => blockType;
        set
        {
            blockType = value;
            block.UpdateBlockType(blockType);
        }
    }

    public PlayerNode TreeNode => treeNode;

    public void SetPlayerNode(PlayerNode node)
    {
        if (treeNode == node)
            return;
        treeNode = node;
        if (node != null)
            block.ChangeAlliance(node.Tree.alliance);
    }
}

public enum BlockType
{
    Dirt,
    Rock,
    Water,
    Twig,
    Leaf,
    DeadTwig
}

public static class BlockGrid
{
    static readonly Dictionary<Vector2Int, BlockInfo> blocks = new();

    static BlockGenerator generator;

    public static void Init(BlockGenerator generator)
    {
        BlockGrid.generator = generator;
        //for (int i = 0; i < 18; i++)
        //{
        //    for (int j = 0; j < 30; j++)
        //    {
        //        generator.GenerateBlock(new Vector2Int(i, -j));
        //    }
        //}

        //PlayerController.Init();
        //GenerateAround(Vector2Int.zero);
    }

    public static void Add(Vector2Int location, BlockInfo info)
    {
        blocks.Add(location, info);
    }

    public static bool Contains(Vector2Int location) => blocks.ContainsKey(location);

    public static bool TryGet(Vector2Int location, out BlockInfo info)
    {
        return blocks.TryGetValue(location, out info);
    }

    public static BlockType GetType(Vector2Int loc) => blocks[loc].BlockType;
    public static PlayerNode GetTreeNode(Vector2Int loc) => blocks[loc].TreeNode;
    public static void SetTwigToLeaf(Vector2Int loc)
    {
        BlockInfo info = blocks[loc];
        if (info.BlockType == BlockType.Twig)
            info.BlockType = BlockType.Leaf;
    }
    public static void SetLeafToTwig(Vector2Int loc)
    {
        BlockInfo info = blocks[loc];
        if (info.BlockType == BlockType.Leaf)
            info.BlockType = BlockType.Twig;
    }
    public static void SetTreeBlockToDirt(Vector2Int loc)
    {
        BlockInfo info = blocks[loc];
        info.SetPlayerNode(null);
        if (info.BlockType == BlockType.Leaf || info.BlockType == BlockType.Twig)
            info.BlockType = BlockType.Dirt;
    }
    public static void ExtendLeaf(Vector2Int from, Vector2Int to, PlayerNode node)
    {
        blocks[from].BlockType = BlockType.Twig;
        BlockInfo newBlock = blocks[to];
        newBlock.SetPlayerNode(node);
        newBlock.BlockType = BlockType.Leaf;
        generator.GenerateLine(to.y - 10);
    }

    // TODO: please Remove
    public static void setPls(Vector2Int loc, BlockType type)
    {
        blocks[loc].BlockType = type;
    }
}
