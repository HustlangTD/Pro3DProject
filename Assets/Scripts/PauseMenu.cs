using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;          // Gán panel pause của bạn vào đây
    public TextMeshProUGUI pauseText;      // Text "Game Paused"
    public Slider volumeSlider;            // Thanh volume
    public Button playAgainButton;         // Nút Play Again
    public Button mainMenuButton;          // Nút Main Menu

    public bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);

        // Gán sự kiện nút
        playAgainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Lấy giá trị volume đã lưu hoặc mặc định
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Dừng toàn bộ game
        pausePanel.SetActive(true);

        // Hiện chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục game
        pausePanel.SetActive(false);

        // Ẩn chuột (tuỳ bạn)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
