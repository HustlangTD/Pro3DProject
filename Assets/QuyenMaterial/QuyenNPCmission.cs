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
    
    private bool isPlayerNear = false;
    private bool missionStarted = false;


    void Update()
    {
        if (isPlayerNear && !missionStarted && Input.GetKeyDown(interactKey))
        {
            StartMission();
        }
    }

    void StartMission()
    {
        missionStarted = true;
        if (interactUI) interactUI.SetActive(false);

        // Kích hoạt cutscene Timeline
        if (timeline != null)
        {
            timeline.Play();
            // Khi timeline kết thúc → gọi hàm mở UI nhiệm vụ
            timeline.stopped += OnTimelineFinished;
        }
        else
        {
            OnTimelineFinished(timeline);
        }
    }

    void OnTimelineFinished(PlayableDirector obj)
    {
        if (missionUI != null)
            missionUI.SetActive(true);
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
}
