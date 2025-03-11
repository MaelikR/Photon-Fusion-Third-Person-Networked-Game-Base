using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform safeZone;

    private HashSet<PlayerRef> spawnedPlayers = new HashSet<PlayerRef>(); // Évite les double spawns

    private void Awake()
    {
        if (safeZone == null) Debug.LogWarning("[PlayerSpawner] Safe zone not assigned in inspector.");
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] PlayerJoined() called for Player {player.PlayerId}");

        if (!IsLocalPlayer(player)) return; // Vérifie si c'est le joueur local
        if (!ValidateComponents()) return;

        if (Runner.TryGetPlayerObject(player, out _))
        {
            Debug.LogWarning($"[PlayerSpawner] Player {player.PlayerId} is already spawned. Skipping.");
            return;
        }

        SpawnPlayer(player);
    }

    private bool ValidateComponents()
    {
        if (Runner == null)
        {
            Debug.LogError("[PlayerSpawner] Runner is null, cannot spawn player.");
            return false;
        }
        if (!playerPrefab.IsValid)
        {
            Debug.LogError("[PlayerSpawner] Player prefab is not assigned or invalid.");
            return false;
        }
        if (safeZone == null)
        {
            Debug.LogError("[PlayerSpawner] Safe zone is not assigned.");
            return false;
        }
        return true;
    }

    private bool IsLocalPlayer(PlayerRef player)
    {
        return player == Runner.LocalPlayer;
    }
    private void SpawnPlayer(PlayerRef player)
    {
        // Utiliser directement la position de la safeZone
        Vector3 spawnPosition = safeZone.position;

        // Assurez-vous que le spawn se fait à une hauteur correcte
        spawnPosition.y = 0f;  // Hauteur assurée au-dessus du sol (ajustez selon vos besoins)

        Debug.Log($"[PlayerSpawner] Trying to spawn player {player.PlayerId} at {spawnPosition}");

        // Spawn du joueur à la position ajustée
        NetworkObject playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        if (playerObject != null)
        {
            Debug.Log($"[PlayerSpawner] Player {player.PlayerId} successfully spawned at {spawnPosition}");
        }
        else
        {
            Debug.LogError($"[PlayerSpawner] Failed to spawn player {player.PlayerId}.");
        }
    }


    private Vector3 ValidateSpawnPosition(Vector3 position)
    {
        int groundLayer = LayerMask.GetMask("Ground");
        RaycastHit hit;

        // Vérifier la position du sol sous la position de spawn calculée
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f, groundLayer))
        {
            position.y = hit.point.y + 1f; // Ajuster pour ne pas spawn directement sur le sol
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] Raycast failed, using default position.");
        }

        return position;
    }

    private Vector3 LoadLastKnownPosition()
    {
        if (PlayerPrefs.HasKey("lastX"))
        {
            Vector3 position = new Vector3(
                PlayerPrefs.GetFloat("lastX"),
                PlayerPrefs.GetFloat("lastY"),
                PlayerPrefs.GetFloat("lastZ")
            );

            Debug.Log($"[PlayerSpawner] Position loaded from PlayerPrefs: {position}");
            return position;
        }

        Debug.Log("[PlayerSpawner] Using safe zone position.");
        return safeZone.position;
    }
}
