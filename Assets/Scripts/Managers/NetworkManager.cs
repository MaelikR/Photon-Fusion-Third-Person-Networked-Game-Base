using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public NetworkRunner networkRunner;

    void Start()
    {
        StartGame(GameMode.Host);
    }

    public void StartGame(GameMode mode)
    {
        if (networkRunner == null)
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
            networkRunner.ProvideInput = true;
        }

        SceneRef sceneRef = networkRunner.SceneManager.GetSceneRef(SceneManager.GetActiveScene().name);

        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "MMORPG_Session",
            Scene = sceneRef, // Utilise la bonne référence de la scène
            PlayerCount = 100
        });
    }


    public void JoinSession()
    {
        StartGame(GameMode.Client);
    }

    public void CreateRoom(string roomName)
    {
        if (networkRunner.LocalPlayer != PlayerRef.None && networkRunner.IsServer)

        // Remplace Object.HasStateAuthority par HasStateAuthority
        {
            Debug.Log("Creating room: " + roomName);
            networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = roomName
            });
        }
    }

    public void LeaveRoom()
    {
        networkRunner.Shutdown();
    }
}
