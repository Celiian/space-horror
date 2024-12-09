using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AnimationData", menuName = "AnimationData/AnimationData")]
public class AnimationData : ScriptableObject
{
	[Serializable]
	public class AnimationEntry
	{
		public string name;
		public float duration;
		public bool isLooping;
		[HideInInspector] public int hash;
	}

	public List<AnimationEntry> animations = new();
	public RuntimeAnimatorController animatorController;
	private Dictionary<string, AnimationEntry> _animationDictionary;

	private void OnEnable()
	{
		_animationDictionary = new Dictionary<string, AnimationEntry>();
		foreach (var anim in animations)
		{
			anim.hash = Animator.StringToHash(anim.name);
			_animationDictionary[anim.name] = anim;
		}
	}

	public int GetHash(string animationName)
	{
		if (_animationDictionary.TryGetValue(animationName, out AnimationEntry entry))
		{
			return entry.hash;
		}
		Debug.LogWarning($"Animation {animationName} not found!");
		return 0;
	}

	public float GetDuration(string animationName)
	{
		if (_animationDictionary.TryGetValue(animationName, out AnimationEntry entry))
		{
			return entry.duration;
		}
		Debug.LogWarning($"Animation {animationName} not found!");
		return 0f;
	}

	public bool IsLooping(string animationName)
	{
		if (_animationDictionary.TryGetValue(animationName, out AnimationEntry entry))
		{
			return entry.isLooping;
		}
		Debug.LogWarning($"Animation {animationName} not found!");
		return false;
	}

	#if UNITY_EDITOR
	public void PopulateAnimationsFromController()
	{
		if (animatorController == null)
		{
			Debug.LogWarning("Animator Controller is not assigned!");
			return;
		}

		animations.Clear();
		var controller = animatorController;
		if (controller != null)
		{
			foreach (var clip in controller.animationClips)
			{
				animations.Add(new AnimationEntry
				{
					name = clip.name,
					duration = clip.length,
					isLooping = clip.isLooping
				});
			}
		}
	}
	#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimationData))]
public class AnimationDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		AnimationData data = (AnimationData) target;

		if (GUILayout.Button("Populate Animations"))
		{
			data.PopulateAnimationsFromController();
			EditorUtility.SetDirty(data);
		}
	}
}
#endif