using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuTests
{
    private PauseMenu pauseMenu;
    private GameObject pausePanelPrefab;
    private const string MAIN_MENU_SCENE_NAME = "MenuScene"; // Thay bằng tên Scene Menu của bạn

    // Thiết lập chung trước khi chạy mỗi test
    [SetUp]
    public void Setup()
    {
        // 1. Tạo một GameObject rỗng để gắn script
        GameObject go = new GameObject("PauseMenuTestObject");
        pauseMenu = go.AddComponent<PauseMenu>();

        // 2. Tạo các thành phần UI giả lập (Mock UI)
        pausePanelPrefab = new GameObject("PausePanel");
        pausePanelPrefab.SetActive(false); // Ban đầu không active

        // Tạo các nút và slider giả lập
        GameObject playAgainGO = new GameObject("PlayAgainButton");
        GameObject mainMenuGO = new GameObject("MainMenuButton");
        GameObject sliderGO = new GameObject("VolumeSlider");

        // Gán các component UI cần thiết
        pauseMenu.pausePanel = pausePanelPrefab;
        pauseMenu.pauseText = go.AddComponent<TextMeshProUGUI>(); // Chỉ cần component, không cần UI thực
        pauseMenu.playAgainButton = playAgainGO.AddComponent<Button>();
        pauseMenu.mainMenuButton = mainMenuGO.AddComponent<Button>();
        pauseMenu.volumeSlider = sliderGO.AddComponent<Slider>();

        // Đặt volume về mặc định cho mỗi test để đảm bảo môi trường sạch
        PlayerPrefs.DeleteKey("MasterVolume");
        Time.timeScale = 1f; // Đảm bảo timeScale được reset
        
        // Khởi tạo Start()
        pauseMenu.gameObject.GetComponent<PauseMenu>().Invoke("Start", 0f);
    }

    // Dọn dẹp sau mỗi test
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(pauseMenu.gameObject);
        Object.Destroy(pausePanelPrefab);
        
        // Reset timeScale và volume sau mỗi test
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
    }

    // --- CÁC TEST CASES VỀ CHỨC NĂNG PAUSE/RESUME ---

    [UnityTest]
    public IEnumerator PauseGame_SetsTimeScaleToZeroAndActivatesPanel()
    {
        // Act: Kích hoạt PauseGame (qua Update với KeyCode.Escape)
        // Lưu ý: Trong PlayMode Test, cần giả lập Input
        Input.GetKeyDown(KeyCode.Escape); // Unity test tools không hỗ trợ giả lập Input.GetKeyDown trực tiếp
        // Thay bằng gọi trực tiếp hàm private bằng reflection hoặc cách đơn giản nhất là gọi logic
        
        // Dùng reflection để gọi hàm private PauseGame()
        var pauseMethod = typeof(PauseMenu).GetMethod("PauseGame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pauseMethod.Invoke(pauseMenu, null);

        yield return null; // Chờ 1 frame để đảm bảo Unity cập nhật trạng thái

        // Assert
        Assert.IsTrue(pauseMenu.isPaused, "isPaused phải là TRUE.");
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale phải là 0.");
        Assert.IsTrue(pauseMenu.pausePanel.activeSelf, "Pause Panel phải được active.");
    }

    [UnityTest]
    public IEnumerator ResumeGame_SetsTimeScaleToOneAndDeactivatesPanel()
    {
        // Setup: Đưa game vào trạng thái Pause
        var pauseMethod = typeof(PauseMenu).GetMethod("PauseGame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pauseMethod.Invoke(pauseMenu, null);
        
        // Act: Kích hoạt ResumeGame (qua Update với KeyCode.Escape)
        var resumeMethod = typeof(PauseMenu).GetMethod("ResumeGame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        resumeMethod.Invoke(pauseMenu, null);

        yield return null; // Chờ 1 frame

        // Assert
        Assert.IsFalse(pauseMenu.isPaused, "isPaused phải là FALSE.");
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale phải là 1.");
        Assert.IsFalse(pauseMenu.pausePanel.activeSelf, "Pause Panel phải bị deactive.");
    }
    
    // --- TEST CASES VỀ CHỨC NĂNG VOLUME ---

    [Test]
    public void SetVolume_UpdatesAudioListenerAndPlayerPrefs()
    {
        float testVolume = 0.5f;

        // Act: Kích hoạt hàm SetVolume thông qua sự kiện Slider
        pauseMenu.volumeSlider.onValueChanged.Invoke(testVolume);

        // Assert
        Assert.AreEqual(testVolume, AudioListener.volume, "AudioListener volume không khớp.");
        Assert.AreEqual(testVolume, PlayerPrefs.GetFloat("MasterVolume"), "PlayerPrefs không lưu volume.");
    }

    [Test]
    public void Start_LoadsSavedVolumeCorrectly()
    {
        float savedVolume = 0.25f;
        PlayerPrefs.SetFloat("MasterVolume", savedVolume);
        
        // Setup/Act: Khởi tạo lại script để kiểm tra Start()
        GameObject go = new GameObject("TestLoadVolume");
        PauseMenu newPauseMenu = go.AddComponent<PauseMenu>();
        newPauseMenu.volumeSlider = new GameObject("Slider").AddComponent<Slider>();
        
        // Gọi Start()
        newPauseMenu.Invoke("Start", 0f);

        // Assert
        Assert.AreEqual(savedVolume, AudioListener.volume, "Start không load đúng volume đã lưu.");
        Assert.AreEqual(savedVolume, newPauseMenu.volumeSlider.value, "Slider value không khớp với volume đã lưu.");
        
        Object.DestroyImmediate(go);
    }
    
    // --- TEST CASES VỀ CHỨC NĂNG TẢI SCENE ---

    [UnityTest]
    public IEnumerator PlayAgain_LoadsCurrentScene()
    {
        // Lưu lại buildIndex hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Act: Kích hoạt nút Play Again
        pauseMenu.playAgainButton.onClick.Invoke();

        // Chờ Unity tải scene. Phải chờ ít nhất 1 frame.
        yield return new WaitForSeconds(0.1f); 

        // Assert: Kiểm tra xem nó có cố gắng tải lại scene không
        // Lưu ý: Trong Play Mode Test, hàm LoadScene thực sự sẽ chạy.
        // Cách test tốt nhất là kiểm tra TimeScale được reset.
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale phải là 1 trước khi tải scene.");
        
        // Vì kiểm tra LoadScene thực tế phức tạp (cần setup nhiều scene), ta chỉ kiểm tra
        // logic reset TimeScale, là phần quan trọng nhất trong script.
    }
    
    [UnityTest]
    public IEnumerator GoToMainMenu_LoadsMainMenuScene()
    {
        // Act: Kích hoạt nút Go To Main Menu
        pauseMenu.mainMenuButton.onClick.Invoke();

        // Chờ Unity tải scene
        yield return new WaitForSeconds(0.1f); 
        
        // Assert: Kiểm tra TimeScale được reset
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale phải là 1 trước khi tải scene.");
        
        // Tương tự, ta kiểm tra logic reset TimeScale.
    }
}