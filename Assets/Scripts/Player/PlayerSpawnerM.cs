using Cinemachine;
using Fusion;
using StarterAssets;
using UnityEngine;

public class PlayerSpawnerM : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab; // Assignez votre prefab de joueur dans l'inspecteur

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.LocalPlayer == player)
        {
            // Spawning the playerPrefab and obtaining the NetworkObject
            NetworkObject networkObject = Runner.Spawn(playerPrefab, new Vector3(328.36f, -8.26f, 23.28f), Quaternion.identity, player);

            // Accessing the GameObject from the NetworkObject
            GameObject spawnedPlayer = networkObject.gameObject;

            // Assign the spawnedPlayer to the ThirdPersonController's instance
            ThirdPersonController thirdPersonController = spawnedPlayer.GetComponent<ThirdPersonController>();
            if (thirdPersonController != null)
            {
                thirdPersonController.spawnedPlayer = spawnedPlayer;
            }

            // If the spawned player has input authority, configure the camera
            if (networkObject.HasInputAuthority)
            {
                var freeLookCamera = spawnedPlayer.GetComponentInChildren<CinemachineFreeLook>();
                if (freeLookCamera != null)
                {
                    freeLookCamera.Follow = spawnedPlayer.transform;
                    freeLookCamera.LookAt = spawnedPlayer.transform;
                    freeLookCamera.enabled = true; // Activate FreeLook camera for the local player
                }
            }
        }
    }
}
