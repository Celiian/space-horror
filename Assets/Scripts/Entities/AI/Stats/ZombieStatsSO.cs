using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Enemy Logic/Stats", fileName = "Stats")]
public sealed class ZombieStatsSO : ScriptableObject
{
	[System.Serializable]
	public class Stat
	{
		public string name;
		public float value;
	}

	[SerializeField]
	private List<Stat> serializedStats = new List<Stat>
	{
		new Stat { name = "speed", value = 4.0f },
		new Stat { name = "slowdownFactor", value = 1f },
		new Stat { name = "attackRange", value = 1.5f },

        new Stat { name = "visionAngle", value = 180f },
        new Stat { name = "visionLength", value = 15f },

		new Stat { name = "hearingDetectionRadius", value = 10f },
		new Stat { name = "hearingVolumeThreshold", value = 0.5f },
        
	};

	private Dictionary<string, float> stats;

	private void OnEnable()
	{
		stats = new Dictionary<string, float>();
		foreach (var stat in serializedStats)
		{
			stats[stat.name] = stat.value;
		}
	}

	public Dictionary<string, float> GetStats() {
		return stats;
	}

	public float GetStat(string statName)
	{
		if (stats.TryGetValue(statName, out float value))
		{
			return (float)value;
		}
		else
		{
			Debug.LogWarning($"Stat {statName} not found!");
			return 0f;
		}
	}

	public void SetStat(string statName, float value)
	{
		if (stats.ContainsKey(statName))
		{
			stats[statName] = value;
		}
		else
		{
			Debug.LogWarning($"Stat {statName} not found!");
		}
	}


	public void ResetStatsToDefault()
	{
		stats = new Dictionary<string, float>(serializedStats.ToDictionary(stat => stat.name, stat => stat.value));
	}
}
