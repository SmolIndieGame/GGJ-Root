using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;

    private void Start()
    {
        image.color = Constants.P1Color;
        button.interactable = false;
        PlayerController.CanEndTurnChange += OnCanEndTurnChange;
        PlayerController.OnEndTurn += OnEndTurn;
    }

    private void OnDestroy()
    {
        PlayerController.CanEndTurnChange -= OnCanEndTurnChange;
        PlayerController.OnEndTurn -= OnEndTurn;
    }

    private void OnEndTurn(int obj)
    {
        image.color = obj == 2 ? Constants.P2Color : Constants.P1Color;
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
