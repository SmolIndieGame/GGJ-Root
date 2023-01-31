using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideBlock : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    public void Setup(Vector2Int location, float score, bool target)
    {
        transform.position = (Vector2)location;
        if (score < 0)
            text.color= Color.red;
        else
            text.color= Color.green;
        text.text = score == 0 ? string.Empty : score.ToString("+#.#;-#.#;0");

        foreach (var item in spriteRenderers)
        {
            item.color = target ? Color.green : Color.yellow;
        }
    }
}
