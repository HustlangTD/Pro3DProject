using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CinemachineCamera camera0; // camera chinh
    public CinemachineCamera camera1; // camera cutsceen

    public void OnCutsceenStart()
    {
        camera0.Priority = 0;
        camera1.Priority = 10;
    }

    public void OnCutsceenEnd()
    {
        camera0.Priority = 10;
        camera1.Priority = 0;
    }
}