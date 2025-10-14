using UnityEngine;
using UnityEngine.SceneManagement;

public class NhanMainMenu : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelSettings;

    // Gọi khi bấm nút "Start Game"
    public void StartGame()
    {
        // Load scene khác, nhớ add scene trong Build Settings
        SceneManager.LoadScene("GameScene");
    }

    // Gọi khi bấm nút "Settings"
    public void OpenSettings()
    {
        panelMainMenu.SetActive(false);
        panelSettings.SetActive(true);
    }

    // Gọi khi bấm "Back" từ settings
    public void BackToMenu()
    {
        panelSettings.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    // Gọi khi bấm "Quit Game"
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
