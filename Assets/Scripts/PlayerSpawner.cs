using System.Numerics;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public NetworkPrefabRef playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        // runner.Spawn(playerPrefab, Vector3.zero, Quarternion.identity)
        if (player == Runner.LocalPlayer)
        {
            var spawnPosition = new UnityEngine.Vector3(UnityEngine.Random.Range(-5f, -5f), 0f, 0f);
            Runner.Spawn(
                playerPrefab,
                spawnPosition,
                quaternion.identity,
                player
            );
        }
    }

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

