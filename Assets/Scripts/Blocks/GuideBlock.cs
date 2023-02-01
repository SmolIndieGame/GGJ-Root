using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideBlock : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text addScore;
    [SerializeField] TMPro.TMP_Text substractScore;
    [SerializeField] GameObject dead;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    public void Setup(Vector2Int location, float score, bool target)
    {
        addScore.enabled = false;
        substractScore.enabled = false;
        dead.SetActive(false);

        transform.position = (Vector2)location;
        foreach (var item in spriteRenderers)
            item.color = target ? Color.cyan : Color.yellow;

        if (score == 0)
            return;

        if (PlayerController.GetCurrentScore() + score < 0)
        {
            dead.SetActive(true);
            return;
        }

        if (score < 0)
        {
            substractScore.enabled = true;
            substractScore.text = score.ToString();
            return;
        }

        addScore.enabled = true;
        addScore.text = $"+{score}";
    }
}
