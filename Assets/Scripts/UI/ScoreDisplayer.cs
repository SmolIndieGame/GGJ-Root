using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int playerNumber;

    void Awake()
    {
        if (playerNumber == 1)
            PlayerController.P1ScoreChange += ScoreChange;
        else
            PlayerController.P2ScoreChange += ScoreChange;
    }

    private void OnDestroy()
    {
        if (playerNumber == 1)
            PlayerController.P1ScoreChange -= ScoreChange;
        else
            PlayerController.P2ScoreChange -= ScoreChange;
    }

    private void ScoreChange(float score)
    {
        text.text = $"P{playerNumber} Water: {score}";
    }
}
