using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    const int Area1Height = 9;
    const int Area2Height = 8;

    [SerializeField] GameObject blockPf;

    [SerializeField] float threshold;
    [SerializeField] float frequency;

    Vector2 noiseOffset;

    bool[,] waterLoc = new bool[BlockGrid.MapWidth, Area1Height + Area2Height];
    public int CurrentHeight { get; private set; }

    //private void OnValidate()
    //{
    //    if (Application.isPlaying)
    //        BlockGrid.Init(this);
    //}

    private void Start()
    {
        for (int i = 0; i < BlockGrid.MapWidth / 3; i++)
            for (int j = 0; j < Area1Height / 3; j++)
                waterLoc[i * 3 + Random.Range(0, 3), j * 3 + Random.Range(0, 3)] = true;

        for (int i = 0; i < BlockGrid.MapWidth / 4; i++)
            for (int j = 0; j < Area2Height / 4; j++)
                waterLoc[i * 4 + Random.Range(0, 4) + (i >= BlockGrid.MapWidth / 8 ? BlockGrid.MapWidth % 4 : 0), j * 4 + Random.Range(0, 4) + Area1Height] = true;

        BlockGrid.Init(this);

        CurrentHeight = 5;
        for (int i = 0; i < BlockGrid.MapWidth; i++)
            for (int j = 0; j < CurrentHeight; j++)
                GenerateBlock(new Vector2Int(i, -j));

        noiseOffset.x = Random.Range(-1000f, 1000f);
        noiseOffset.y = Random.Range(-1000f, 1000f);

        PlayerController.Init();

        AudioManager.I.Play("Battle");
    }

    public void GenerateLine(int y)
    {
        for (; CurrentHeight < -y; CurrentHeight++)
        {
            for (int i = 0; i < BlockGrid.MapWidth; i++)
                GenerateBlock(new Vector2Int(i, -CurrentHeight));
        }
    }

    void GenerateBlock(Vector2Int location)
    {
        if (BlockGrid.Contains(location))
            return;

        BlockType blockType;
        if (location.y == 0 || PlayerController.deepWaterLocations.Contains(location))
            blockType = BlockType.Dirt;
        else if (-location.y < Area1Height + Area2Height && waterLoc[location.x, -location.y])
            blockType = BlockType.Water;
        else
        {
            var f = Mathf.PerlinNoise(location.x * frequency + noiseOffset.x, location.y * frequency + noiseOffset.y);
            if (f < threshold)
                blockType = BlockType.Dirt;
            else
                blockType = location.y > -2 ? BlockType.Dirt : BlockType.Rock;
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
