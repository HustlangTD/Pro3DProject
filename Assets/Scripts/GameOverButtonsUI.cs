using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverButtonsUI : MonoBehaviour
{
    public Button tryAgainButton;
    public Button mainMenuButton;

    private void Start()
    {
        // Gán sự kiện click
        tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnTryAgainClicked()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void OnMainMenuClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
