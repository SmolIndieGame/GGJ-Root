using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public static BlockGenerator I { get; private set; }

    [SerializeField] GameObject blockPf;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        BlockGrid.Init();
    }

    public void GenerateBlock(Vector2Int location)
    {
        if (BlockGrid.Contains(location))
            return;

        BlockType blockType;
        var f = Random.value;
        if (f < 0.7f)
            blockType = BlockType.Dirt;
        else if (f < 0.9f)
            blockType = BlockType.Rock;
        else
            blockType = BlockType.Water;

        var block = new BlockInfo();
        var obj = Instantiate(blockPf).GetComponent<Block>();
        obj.Setup(GetColor(blockType), location);

        block.location = location;
        block.blockType = blockType;
        block.block = obj;
        BlockGrid.Add(location, block);
    }

    Color GetColor(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Dirt => Color.white,
            BlockType.Rock => Color.gray,
            BlockType.Water => Color.cyan,
            BlockType.Root => Color.yellow,
            _ => Color.white,
        };
    }
}
