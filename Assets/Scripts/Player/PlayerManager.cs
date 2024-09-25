using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} has joined the game.");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} has left the game.");

        // Récupérez le NetworkObject associé au joueur déconnecté
        NetworkObject playerObject = runner.GetPlayerObject(player);

        if (playerObject != null && playerObject.IsValid)
        {
            // Nettoyez les objets du joueur qui a quitté le jeu
            runner.Despawn(playerObject);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason disconnectReason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
}
