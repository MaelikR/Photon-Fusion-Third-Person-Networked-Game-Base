
using Fusion;
using StarterAssets;
using UnityEngine;
namespace Unity.Cinemachine
{
    public class PlayerSpawnerM : SimulationBehaviour, IPlayerJoined
    {
        public GameObject playerPrefab; // Assignez votre prefab de joueur dans l'inspecteur

        public void PlayerJoined(PlayerRef player)
        {
            if (Runner.LocalPlayer == player)
            {
                // Spawning the playerPrefab and obtaining the NetworkObject
                NetworkObject networkObject = Runner.Spawn(playerPrefab, new Vector3(139.7912f, -7.126f, 138.6414f), Quaternion.identity, player);

                // Accessing the GameObject from the NetworkObject
                GameObject spawnedPlayer = networkObject.gameObject;

                // Assign the spawnedPlayer to the ThirdPersonController's instance
                ThirdPersonController thirdPersonController = spawnedPlayer.GetComponent<ThirdPersonController>();
                if (thirdPersonController != null)
                {
                    thirdPersonController.spawnedPlayer = spawnedPlayer;
                }
            }
        }
    }
}