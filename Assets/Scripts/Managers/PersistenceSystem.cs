using Fusion;
using System.Diagnostics;
using System;
using System.IO;
using UnityEngine;


public class PersistenceSystem : NetworkBehaviour
{
	public CharacterStats characterStats;

	public void SaveData()
	{
		if (Object.HasStateAuthority)
		{
			string path = Application.persistentDataPath + "/saveData.json";
			string jsonData = JsonUtility.ToJson(characterStats);
			File.WriteAllText(path, jsonData);
			UnityEngine.Debug.Log("Data saved.");
		}
	}

	public void LoadData()
	{
		if (Object.HasStateAuthority)
		{
			string path = Application.persistentDataPath + "/saveData.json";
			if (File.Exists(path))
			{
				string jsonData = File.ReadAllText(path);
				JsonUtility.FromJsonOverwrite(jsonData, characterStats);
				UnityEngine.Debug.Log("Data loaded.");
			}
		}
	}
}
