using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepWaterLocations : MonoBehaviour
{
    private void Awake()
    {
        PlayerController.deepWaterLocations.Clear();
        foreach (Transform child in transform)
        {
            Vector2Int pos = new((int)child.position.x, (int)child.position.y);
            PlayerController.deepWaterLocations.Add(pos);
        }
    }
}
