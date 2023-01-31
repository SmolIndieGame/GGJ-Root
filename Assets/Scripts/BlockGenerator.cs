using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    const int MapWidth = 18;
    const int Area1Height = 12;
    const int Area2Height = 12;

    [SerializeField] GameObject blockPf;

    [SerializeField] float threshold;
    [SerializeField] float frequency;

    Vector2 noiseOffset;

    bool[,] waterLoc = new bool[MapWidth, Area1Height + Area2Height];
    int currentHeight;

    //private void OnValidate()
    //{
    //    if (Application.isPlaying)
    //        BlockGrid.Init(this);
    //}

    private void Start()
    {
        for (int i = 0; i < MapWidth / 3; i++)
            for (int j = 0; j < Area1Height / 3; j++)
                waterLoc[i * 3 + Random.Range(0, 3), j * 3 + Random.Range(0, 3)] = true;

        for (int i = 0; i < MapWidth / 4; i++)
            for (int j = 0; j < Area2Height / 4; j++)
                waterLoc[i * 4 + Random.Range(0, 4) + MapWidth % 4 / 2, j * 4 + Random.Range(0, 4) + Area1Height] = true;

        currentHeight = Area1Height + Area2Height;
        for (int i = 0; i < MapWidth; i++)
            for (int j = 0; j < currentHeight; j++)
                GenerateBlock(new Vector2Int(i, -j));

        noiseOffset.x = Random.Range(-1000f, 1000f);
        noiseOffset.y = Random.Range(-1000f, 1000f);

        BlockGrid.Init(this);
        PlayerController.Init();
    }

    public void GenerateLine(int y)
    {
        for (; currentHeight < -y; currentHeight++)
        {
            for (int i = 0; i < MapWidth; i++)
                GenerateBlock(new Vector2Int(i, -currentHeight));
        }
    }

    void GenerateBlock(Vector2Int location)
    {
        if (BlockGrid.Contains(location))
            return;

        BlockType blockType;
        if (-location.y < Area1Height + Area2Height && waterLoc[location.x, -location.y])
            blockType = BlockType.Water;
        else
        {
            var f = Mathf.PerlinNoise(location.x * frequency + noiseOffset.x, location.y * frequency + noiseOffset.y);
            if (f < threshold)
                blockType = BlockType.Dirt;
            else
                blockType = BlockType.Rock;
        }

        //if (BlockGrid.Contains(location))
        //{
        //    BlockGrid.setPls(location, blockType);
        //    return;
        //}

        var obj = Instantiate(blockPf).GetComponent<VisualBlock>();
        obj.Setup(location);
        var block = new BlockInfo(location, blockType, obj);
        BlockGrid.Add(location, block);
    }
}
