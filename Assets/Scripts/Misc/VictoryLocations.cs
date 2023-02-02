using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryLocations : MonoBehaviour
{
    [SerializeField] Transform[] locs;

    private void Awake()
    {
        PlayerController.victoryLocations.Clear();
        foreach (var loc in locs)
        {
            Vector2Int pos = new((int)loc.position.x, (int)loc.position.y);
            PlayerController.victoryLocations.Add(pos);
        }
    }
}
