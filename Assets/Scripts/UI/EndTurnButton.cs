using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;

    static Color a1 = Color.red;
    static Color a2 = Color.blue;

    private void Start()
    {
        image.color = a1;
        button.interactable = false;
        PlayerController.CanEndTurnChange += OnCanEndTurnChange;
        PlayerController.onEndTurn += OnEndTurn;
    }

    private void OnEndTurn(int obj)
    {
        image.color = obj == 2 ? a2 : a1;
    }

    private void OnCanEndTurnChange(bool ableToEndTurn)
    {
        button.interactable = ableToEndTurn;
    }

    public void OnClick()
    {
        PlayerController.EndCurrentPlayerTurn();
    }
}
