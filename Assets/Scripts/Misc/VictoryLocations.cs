using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryLocations : MonoBehaviour
{
    private void Awake()
    {
        PlayerController.victoryLocations.Clear();
        foreach (Transform child in transform)
        {
            Vector2Int pos = new((int)child.position.x, (int)child.position.y);
            PlayerController.victoryLocations.Add(pos);
        }
    }
}
