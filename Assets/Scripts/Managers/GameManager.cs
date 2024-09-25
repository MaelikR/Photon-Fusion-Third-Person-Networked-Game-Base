using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPrefabRef playerPrefab; // Assign your player prefab in the inspector
    private NetworkRunner _networkRunner;

    async void Start()
    {
        // Create and start a new NetworkRunner
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
        _networkRunner.ProvideInput = true; // Enable if using input authority

        // Start the game
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient, // You can change the mode as needed
            SessionName = "TestSession",          // Name of the session
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await _networkRunner.StartGame(startGameArgs);

        if (!result.Ok)
        {
            Debug.LogError("Failed to start game: " + result.ShutdownReason);
        }
        else
        {
            Debug.Log("Game started successfully.");
            _networkRunner.AddCallbacks(this); // Register the callbacks for INetworkRunnerCallbacks
        }
    }

    // Called when a player joins the game
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Spawn the player prefab at a random position when a new player joins
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
            runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        }
    }

    // Cleanup when a player leaves
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var networkObject = runner.GetPlayerObject(player);
            if (networkObject != null)
            {
                runner.Despawn(networkObject);
            }
        }
    }

    // You can add custom input handling here if needed
    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
