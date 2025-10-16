using UnityEngine;

public class BoatTrigger : MonoBehaviour
{
    // gán manager trong inspector (tốt hơn là gán)
    public NhanIntroManager introManager;

    void Reset()
    {
        // cố gắng tự gán manager nếu có trong scene
        if (introManager == null)
            introManager = FindObjectOfType<NhanIntroManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Assuming Player has tag "Player"
        if (other.CompareTag("Player"))
        {
            if (introManager != null)
            {
                introManager.OnBoatReached();
            }
            else
            {
                Debug.LogWarning("[BoatTrigger] introManager chưa gán!");
            }
        }
    }
}
