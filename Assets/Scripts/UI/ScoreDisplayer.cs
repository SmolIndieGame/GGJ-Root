using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;

    void Update()
    {
        text.text = BlockGrid.score.ToString();
    }
}
