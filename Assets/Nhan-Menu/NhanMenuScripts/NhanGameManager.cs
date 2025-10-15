using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NhanGameManager : MonoBehaviour
{
    public static NhanGameManager Instance;

    [Header("Setting Panel")]
    public GameObject settingPanel;
    public Slider volumeSlider;
    public AudioSource bgmAudio;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ suốt các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ToggleSetting()
    {
        if (settingPanel != null)
            settingPanel.SetActive(!settingPanel.activeSelf);
    }

    public void ChangeVolume(float value)
    {
        if (bgmAudio != null)
            bgmAudio.volume = value;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
