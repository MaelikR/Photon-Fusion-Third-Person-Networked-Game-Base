using UnityEngine;

public class LightingInitializer : MonoBehaviour
{
    public Material skyboxMaterial;
    public Light sunLight;

    void Start()
    {
        // Re-enable the skybox
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }

        // Re-enable the sun
        if (sunLight != null)
        {
            RenderSettings.sun = sunLight;
        }

        // Re-enable fog if needed
        RenderSettings.fog = true;
    }
}
