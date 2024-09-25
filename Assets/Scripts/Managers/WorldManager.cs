using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : NetworkBehaviour
{
    public string currentZone { get; set; }
    public string[] worldZones;

    // Add references for lighting settings
    public Material skyboxMaterial;
    public Light sunLight;
    public bool enableFog = true;

    public void LoadZone(string zoneName)
    {
        if (Object.HasStateAuthority && !SceneManager.GetSceneByName(zoneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(zoneName, LoadSceneMode.Additive);
            currentZone = zoneName;
            RPC_UpdateZone(currentZone);

            // Restore lighting settings after the scene loads
            Invoke(nameof(ApplyLightingSettings), 1.0f); // Delay to ensure scene is loaded
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateZone(string zoneName)
    {
        UnityEngine.Debug.Log("Loading zone: " + zoneName);
    }

    public void UnloadZone(string zoneName)
    {
        if (SceneManager.GetSceneByName(zoneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(zoneName);
            UnityEngine.Debug.Log("Unloading zone: " + zoneName);
        }
    }

    public void TravelToZone(string newZone)
    {
        UnloadZone(currentZone);
        LoadZone(newZone);
    }

    private void ApplyLightingSettings()
    {
        // Apply skybox
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }

        // Apply sun light
        if (sunLight != null)
        {
            RenderSettings.sun = sunLight;
        }

        // Apply fog settings
        RenderSettings.fog = enableFog;
    }
}
