using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneHandler : MonoBehaviour
{
    private void Start()
    {
        AudioManager.I.Play("Title");
    }

    public void LoadToBattle() => SceneManager.LoadScene("Battle");
}
