using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class QuyenNPCmission : MonoBehaviour
{
    public string[] dialogtext = new[]
    {
        "Nghe đây, lính! Khu vực số 7 bị bọn quái tràn vào." ,
        "Hiện tại không rõ số lượng, nhưng radar ghi nhận",
        "ít nhất mười mục tiêu đang di chuyển.",
        "Nhiệm vụ của cậu là vào đó, dọn sạch khu vực. Không để con nào sống sót.",
        "Âm thầm hành động. Đừng gây chú ý, và nhớ đạn của cậu",
        "là thứ duy nhất giữa chúng và chúng ta.",
        "Hoàn thành nhiệm vụ, tôi sẽ nâng cấp vũ khí cho cậu."
    };

    public TextMeshProUGUI dialogTextMesh;

    public int currentIndex;

    private void Start()
    {
        currentIndex = 0;
        dialogtext = new[]
        {
            "Nghe đây, lính! Khu vực số 7 bị bọn quái tràn vào." ,
            "Hiện tại không rõ số lượng, nhưng radar ghi nhận",
            "ít nhất mười mục tiêu đang di chuyển.",
            "Nhiệm vụ của cậu là vào đó, dọn sạch khu vực. Không để con nào sống sót.",
            "Âm thầm hành động. Đừng gây chú ý, và nhớ đạn của cậu",
            "là thứ duy nhất giữa chúng và chúng ta.",
            "Hoàn thành nhiệm vụ, tôi sẽ nâng cấp vũ khí cho cậu."
        };

        //UI nhiem vu
        if (interactUI) interactUI.SetActive(false);
        if (missionUI) missionUI.SetActive(false);
        if (skipUI) skipUI.SetActive(false);
    }

    public void Speak()
    {
        StartCoroutine(SpeakEnum());
    }
    IEnumerator SpeakEnum()
    {

        dialogTextMesh.text = "";
        foreach (var ch in dialogtext[currentIndex])
        {
            dialogTextMesh.text += ch;
            yield return new WaitForSeconds(0.05f);
        }
        currentIndex++;

    }

    //UI nhiem vu
    [Header("Player & UI")]
    public GameObject interactUI;      // UI "Nhấn E để nói chuyện"
    public KeyCode interactKey = KeyCode.E;

    [Header("Cutscene & Mission")]
    public PlayableDirector timeline;  // Timeline cutscene
    public GameObject missionUI;       // UI nhiệm vụ (xuất hiện sau cutscene)
    public GameObject skipUI;            // UI nút skip cutscene (Canvas nhỏ có nút Skip)
    public KeyCode skipKey = KeyCode.Space;

    private bool isPlayerNear = false;
    private bool missionStarted = false;
    private bool cutscenePlaying = false;

    [Header("Player Settings")]
    public GameObject player;            
    
    // 🔸 Thêm mới: các thành phần điều khiển player để tắt/bật khi cutscene chạy
    public MonoBehaviour[] playerControlScripts; // VD: PlayerMovement, ThirdPersonController
    public CharacterController playerController; // nếu có
    public Animator playerAnimator;              // nếu có
    public GameObject playerCamera;              // nếu có camera riêng (VD: Cinemachine)

    void Update()
    {
        if (isPlayerNear && !missionStarted && Input.GetKeyDown(interactKey))
        {
            StartMission();
            // playerMovement.cutscenePlaying = true;
        }
        // Nếu đang phát timeline → cho phép skip
        if (cutscenePlaying && (Input.GetKeyDown(skipKey)))
        {
            SkipCutscene();
        }
    }

    void StartMission()
    {
        missionStarted = true;
        if (interactUI) interactUI.SetActive(false);

        // 🔸 Thêm mới: Khóa điều khiển nhân vật khi cutscene bắt đầu
        LockPlayerControl(true);

        // Kích hoạt cutscene Timeline
        if (timeline != null)
        {
            timeline.Play();

            cutscenePlaying = true;

            if (skipUI) skipUI.SetActive(true);

            // Khi timeline kết thúc → gọi hàm mở UI nhiệm vụ
            timeline.stopped += OnTimelineFinished;
            // playerMovement.cutscenePlaying = false;
        }
        else
        {
            OnTimelineFinished(timeline);
        }
    }

    public void SkipCutscene()
    {
        if (timeline != null)
        {
            timeline.time = timeline.duration;  // nhảy tới cuối timeline
            timeline.Evaluate();                // cập nhật khung hình cuối
            timeline.Stop();
            // playerMovement.cutscenePlaying = false;
        }
    }

    void OnTimelineFinished(PlayableDirector obj)
    {
        if (missionUI != null)
            missionUI.SetActive(true);

        // 🔸 Thêm mới: Mở lại điều khiển nhân vật sau khi cutscene kết thúc
        LockPlayerControl(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (!missionStarted && interactUI)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactUI)
                interactUI.SetActive(false);
        }
    }
    
    // 🔸 Thêm mới: Hàm tắt/bật toàn bộ điều khiển nhân vật
    void LockPlayerControl(bool lockState)
    {
        // Bật/tắt các script điều khiển
        if (playerControlScripts != null)
        {
            foreach (var s in playerControlScripts)
            {
                if (s != null) s.enabled = !lockState;
            }
        }

        // Tắt CharacterController nếu có
        if (playerController != null)
            playerController.enabled = !lockState;

        // Dừng Animator (nhân vật đứng yên)
        if (playerAnimator != null)
            playerAnimator.speed = lockState ? 0 : 1;

        // Tắt camera điều khiển nếu có (VD: Cinemachine)
        if (playerCamera != null)
            playerCamera.SetActive(!lockState);
    }

}
