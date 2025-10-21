using System.Collections.Generic;
using UnityEngine;

public class DistanceActivator : MonoBehaviour
{
    [Header("References")]
    public Transform player;                      // Gán Player (Transform)
    public List<GameObject> objectsToToggle;      // Danh sách các object cần ẩn/hiện

    [Header("Settings")]
    public float activationDistance = 10f;        // Khoảng cách để hiện object
    public float checkInterval = 0.5f;            // Tần suất kiểm tra (đỡ tốn hiệu năng)

    private void Start()
    {
        // Ẩn tất cả ngay từ đầu nếu ở xa
        StartCoroutine(CheckDistanceRoutine());
    }

    private System.Collections.IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj == null) continue;

                float distance = Vector3.Distance(player.position, obj.transform.position);

                bool shouldBeActive = distance <= activationDistance;

                if (obj.activeSelf != shouldBeActive)
                    obj.SetActive(shouldBeActive);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // Vẽ phạm vi trong Scene cho dễ thấy
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, activationDistance);
        }
    }
}
