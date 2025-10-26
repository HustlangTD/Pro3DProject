using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Playables; // 🎬 Dùng cho Timeline

public class NhanIntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelIntro;
    public GameObject panelSetting;

    [Header("UI Buttons")]
    public Button skipButton;
    public Button backButton;
    public Slider volumeSlider;

    [Header("Subtitle")]
    public TextMeshProUGUI subtitleText;
    public Image introImage;
    public Sprite[] introImages;

    [Header("Scene Objects")]
    public GameObject player;
    public AudioSource bgMusic;
    public Camera mainCamera;

    [Header("Voice Settings (AI Voice Offline)")]
    public AudioSource introVoice;
    public AudioClip introClip;

    [Header("Timeline Cutscene")]
    public PlayableDirector introTimeline; // 🎬 Gắn Playable Director vào đây

    private bool isIntroPlaying = false;
    private float defaultVolume = 0.5f; // ✅ Volume mặc định 50%

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
        // ✅ Ngăn Timeline tự chạy khi bắt đầu
        if (introTimeline != null)
            introTimeline.playOnAwake = false;

        // ✅ Đảm bảo chỉ có 1 EventSystem
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
        {
            for (int i = 1; i < systems.Length; i++)
                Destroy(systems[i].gameObject);
        }

        // ✅ Ẩn các panel ban đầu
        if (panelSetting) panelSetting.SetActive(false);
        if (player) player.SetActive(false);

        // ✅ Gán sự kiện
        if (skipButton) skipButton.onClick.AddListener(SkipIntro);
        if (backButton) backButton.onClick.AddListener(CloseSetting);
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(ChangeVolume);

        // ✅ Thiết lập volume mặc định
        if (volumeSlider) volumeSlider.value = defaultVolume;
        if (bgMusic)
        {
            bgMusic.volume = defaultVolume;
            if (!bgMusic.isPlaying) bgMusic.Play();
        }
        if (introVoice) introVoice.volume = defaultVolume;

        if (mainCamera == null) mainCamera = Camera.main;

        // ✅ Bắt đầu intro
        StartCoroutine(PlayIntro());
        StartCoroutine(SceneAutoTimeout());
    }

    void Update()
    {
        // Xoay camera nhẹ trong intro cho sinh động
        if (isIntroPlaying && mainCamera)
        {
            mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, 5f * Time.deltaTime);
        }
    }

    IEnumerator PlayIntro()
    {
        isIntroPlaying = true;
        if (panelIntro) panelIntro.SetActive(true);

        // ✅ Phát voice
        if (introVoice && introClip)
        {
            introVoice.clip = introClip;
            introVoice.Play();
        }

        // ✅ Hiển thị từng dòng intro + hình
        for (int i = 0; i < introLines.Length; i++)
        {
            if (subtitleText) subtitleText.text = introLines[i];
            if (introImage && introImages != null && i < introImages.Length)
                introImage.sprite = introImages[i];

            yield return new WaitForSeconds(4f);
        }

        // ✅ Đợi voice phát xong hoàn toàn
        if (introVoice && introVoice.isPlaying)
        {
            Debug.Log("🎧 Đợi voice clip phát xong...");
            yield return new WaitWhile(() => introVoice.isPlaying);
        }

        // ✅ Kết thúc intro
        isIntroPlaying = false;

        // Sau intro → chạy Timeline
        StartCoroutine(PlayTimelineThenLoadScene());
    }

    public void SkipIntro()
    {
        StopAllCoroutines();

        if (introVoice && introVoice.isPlaying)
            introVoice.Stop();

        if (introTimeline && introTimeline.state == PlayState.Playing)
            introTimeline.Stop();

        // 🟢 Hiện hình cuối cùng vài giây trước khi chạy timeline
        if (panelIntro) panelIntro.SetActive(true);
        if (introImage && introImages.Length > 0)
            introImage.sprite = introImages[introImages.Length - 1];
        if (subtitleText) subtitleText.text = "Nhiệm vụ đầu tiên… cũng có thể là cuối cùng.";

        Debug.Log("⏩ Skip intro → Hiện hình cuối cùng → Chuẩn bị chạy Timeline");
        StartCoroutine(PlayTimelineThenLoadScene());
    }

    IEnumerator PlayTimelineThenLoadScene()
    {
        yield return new WaitForSeconds(2f); // Giữ hình cuối 2 giây

        // 🟢 Ẩn hình & text trước khi Timeline bắt đầu
        if (panelIntro) panelIntro.SetActive(false);
        if (subtitleText) subtitleText.text = "";
        if (introImage) introImage.enabled = false;

        // 🎬 Chạy Timeline
        if (introTimeline != null)
        {
            Debug.Log("🎬 Bắt đầu phát Timeline...");
            introTimeline.Play();
            yield return new WaitUntil(() => introTimeline.state != PlayState.Playing);
            Debug.Log("✅ Timeline kết thúc → Chuyển Scene");
        }

        // ✅ Sau khi Timeline kết thúc → qua scene chính
        SceneManager.LoadScene("Scene 5");
    }

    public void ChangeVolume(float value)
    {
        if (bgMusic) bgMusic.volume = value;
        if (introVoice) introVoice.volume = value;
    }

    public void CloseSetting()
    {
        if (panelSetting) panelSetting.SetActive(false);
    }

    IEnumerator SceneAutoTimeout()
    {
        yield return new WaitForSeconds(180f);
        Debug.Log("🕒 Hết 180s → Tự qua scene chính");
        SceneManager.LoadScene("Scene 5");
    }
}
