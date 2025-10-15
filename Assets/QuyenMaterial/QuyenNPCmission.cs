using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
}
