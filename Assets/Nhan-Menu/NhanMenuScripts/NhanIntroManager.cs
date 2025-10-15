using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class NhanIntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelMenu;
    public GameObject panelIntro;
    public GameObject panelGuide;
    public GameObject panelSetting;

    [Header("UI Buttons")]
    public Button startButton;
    public Button skipButton;
    public Button settingButton;
    public Button backButton;     // 🆕 Nút Back trong Setting Panel
    public Button quitButton;
    public Slider volumeSlider;

    [Header("Subtitle")]
    public TextMeshProUGUI subtitleText;
    public Image introImage;
    public Sprite[] introImages;

    [Header("Scene Objects")]
    public GameObject player;
    public AudioSource bgMusic;
    public Camera mainCamera;
    public GameObject boat;

    [Header("Voice Settings (AI Voice Offline)")]
    public AudioSource introVoice;
    public AudioClip introClip;

    private bool isIntroPlaying = false;

    string[] introLines = new string[]
    {
        "Năm 2025...",
        "Trên hòn đảo Kojo – trung tâm nghiên cứu sinh học tuyệt mật...",
        "Một cuộc bạo loạn kinh hoàng đã bùng nổ.",
        "Các sinh vật thí nghiệm nổi dậy, tiêu diệt toàn bộ đội ngũ nghiên cứu.",
        "Chính phủ buộc phải phong tỏa hòn đảo.",
        "Và phái quân đội đến để dập tắt mối đe dọa này.",
        "Trong số những người lính được điều đi, có một tân binh – chính là bạn.",
        "Nhiệm vụ đầu tiên… cũng có thể là cuối cùng."
    };

    void Start()
    {
        // Ẩn/hiện panel mặc định
        panelMenu?.SetActive(true);
        panelIntro?.SetActive(false);
        panelGuide?.SetActive(false);
        panelSetting?.SetActive(false);
        if (player) player.SetActive(false);

        // Gán sự kiện cho các nút
        if (startButton) startButton.onClick.AddListener(StartGame);
        if (skipButton) skipButton.onClick.AddListener(SkipIntro);
        if (settingButton) settingButton.onClick.AddListener(OpenSetting);
        if (backButton) backButton.onClick.AddListener(CloseSetting); // 🆕
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(ChangeVolume);

        if (bgMusic != null)
            bgMusic.volume = volumeSlider ? volumeSlider.value : 1f;

        if (mainCamera == null)
            mainCamera = Camera.main;
        if (boat == null)
            boat = GameObject.FindWithTag("Boat");
    }

    // 👉 Khi bấm Start
    public void StartGame()
    {
        panelMenu.SetActive(false);
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        isIntroPlaying = true;
        panelIntro.SetActive(true);

        // Phát giọng đọc AI (nếu có)
        if (introVoice && introClip)
        {
            introVoice.clip = introClip;
            introVoice.Play();
        }

        // Hiển thị từng dòng intro (5 giây mỗi lần)
        for (int i = 0; i < introLines.Length; i++)
        {
            subtitleText.text = introLines[i];
            if (introImages != null && i < introImages.Length && introImage != null)
                introImage.sprite = introImages[i];

            if (mainCamera && boat)
                mainCamera.transform.RotateAround(boat.transform.position, Vector3.up, 10 * Time.deltaTime);

            yield return new WaitForSeconds(5f);
        }

        panelIntro.SetActive(false);
        StartCoroutine(ShowGuide());
    }

    IEnumerator ShowGuide()
    {
        panelGuide.SetActive(true);
        if (player) player.SetActive(true);

        TextMeshProUGUI guideText = panelGuide.GetComponentInChildren<TextMeshProUGUI>();
        if (guideText != null)
        {
            guideText.text = "Di chuyển: W A S D";
            yield return new WaitForSeconds(2f);
            guideText.text = "Nhảy: Space";
            yield return new WaitForSeconds(2f);
            guideText.text = "Bắn: Chuột trái";
            yield return new WaitForSeconds(3f);
        }

        panelGuide.SetActive(false);
        isIntroPlaying = false;
    }

    // 🟢 Mở panel Setting
    public void OpenSetting()
    {
        panelSetting.SetActive(true);
        panelMenu.SetActive(false);
    }

    // 🔴 Đóng panel Setting (Back)
    public void CloseSetting()
    {
        panelSetting.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void SkipIntro()
    {
        StopAllCoroutines();
        if (introVoice && introVoice.isPlaying)
            introVoice.Stop();

        panelIntro.SetActive(false);
        StartCoroutine(ShowGuide());
        Debug.Log("⏩ Đã bỏ qua intro");
    }

    public void ChangeVolume(float value)
    {
        if (bgMusic) bgMusic.volume = value;
        if (introVoice) introVoice.volume = value;
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game...");
        Application.Quit();
    }
}
