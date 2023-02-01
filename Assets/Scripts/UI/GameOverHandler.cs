using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TMPro.TMP_Text title;
    [SerializeField] Image fade;

    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeDelay;

    double startTime;

    private void Start()
    {
        PlayerController.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        PlayerController.OnGameOver -= OnGameOver;
    }

    private void Update()
    {
        if (Time.timeAsDouble > startTime + fadeDelay && fade.gameObject.activeInHierarchy)
        {
            fade.color = new Color(0, 0, 0, Mathf.MoveTowards(fade.color.a, 0, fadeSpeed * Time.deltaTime));
            if (fade.color.a < 0.01f)
                fade.gameObject.SetActive(false);
        }
    }

    private void OnGameOver(int player)
    {
        title.text = $"Player{player} Won!";
        gameOverScreen.SetActive(true);
        AudioManager.I.StopAll();
        AudioManager.I.Play("GameOver");

        startTime = Time.timeAsDouble;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Battle");
    }
}
