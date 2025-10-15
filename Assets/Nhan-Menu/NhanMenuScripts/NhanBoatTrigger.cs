using UnityEngine;
using UnityEngine.SceneManagement;

public class NhanBoatTrigger : MonoBehaviour
{
    public string nextScene = "MainScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
