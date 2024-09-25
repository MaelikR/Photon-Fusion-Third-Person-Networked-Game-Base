using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;

public class WeatherSystem : NetworkBehaviour
{

	public float dayDuration = 120f; // Dur�e d'un cycle jour/nuit en secondes
	public float timeOfDay = 0f;
	public bool isDaytime = true;

	public ParticleSystem rainEffect;
	public ParticleSystem snowEffect;

    public Light sun;

    void Start()
    {
        if (sun == null)
        {
            GameObject sunObject = GameObject.Find("Sun");
            if (sunObject != null)
            {
                sun = sunObject.GetComponent<Light>();
                if (sun == null)
                {
                    UnityEngine.Debug.LogError("L'objet 'Sun' existe, mais il ne poss�de pas de composant Light. Veuillez ajouter un composant Light.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Objet 'Sun' non trouv� ! Assurez-vous que l'objet existe dans la sc�ne.");
            }
        }

        // Assurez-vous que 'Object' ne soit pas assign� s'il est en lecture seule.
        // L'initialisation de 'Object' devrait �tre faite via d'autres moyens si n�cessaire.
    }


    void Update()
    {
        // Toujours augmenter le temps de la journ�e ind�pendamment de l'autorit�
        timeOfDay += (Time.deltaTime / dayDuration);

        // Si la r�f�rence au soleil est valide, appliquez la rotation
        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(new Vector3((timeOfDay * 360f) - 90f, 170f, 0f));
        }

        // R�initialiser le temps de la journ�e apr�s un cycle complet
        if (timeOfDay >= 1)
        {
            timeOfDay = 0;
            isDaytime = !isDaytime;

            if (Object != null && Object.HasStateAuthority) // L'autorit� informe les autres du changement de cycle jour/nuit
            {
                RPC_NotifyDayCycleChange(isDaytime);
            }
        }
    }


    public void ChangeWeather(string weatherType)
	{
		if (Object.HasStateAuthority)
		{
			if (weatherType == "rain")
			{
				rainEffect.Play();
				snowEffect.Stop();
			}
			else if (weatherType == "snow")
			{
				snowEffect.Play();
				rainEffect.Stop();
			}
			else
			{
				rainEffect.Stop();
				snowEffect.Stop();
			}
			RPC_NotifyWeatherChange(weatherType);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyDayCycleChange(bool isDay)
	{
		UnityEngine.Debug.Log("It is now " + (isDay ? "daytime" : "nighttime"));
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_NotifyWeatherChange(string weatherType)
	{
		UnityEngine.Debug.Log("Weather changed to " + weatherType);
	}
}
