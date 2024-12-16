using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class StorySoundManager : MonoBehaviour
{
    [Serializable]
    public class StorySound
    {
        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings", CenterLabel = true)]
        [LabelWidth(100)]
        public AudioClip SoundClip;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public bool PropagateSound;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public GameObject PropagationSource;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/Trigger Settings", CenterLabel = true)]
        [LabelWidth(100)]
        [LabelText("Trigger Type")]
        [ValueDropdown(nameof(TriggerTypeOptions))]
        public TriggerType Trigger;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/Trigger Settings")]
        [ShowIf(nameof(IsColliderTrigger))]
        [Tooltip("Specify a GameObject with a collider.")]
        public GameObject TriggerObject;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/Trigger Settings")]
        [ShowIf(nameof(IsTimeTrigger))]
        [Tooltip("Time in seconds after the scene starts.")]
        [LabelWidth(100)]
        public float TriggerTime;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/Trigger Settings")]
        [ShowIf(nameof(IsCustomEventTrigger))]
        [Tooltip("Event to trigger this sound.")]
        [LabelWidth(120)]
        public string CustomEventName;

        [HideInInspector]
        public bool Triggered;

        [BoxGroup("Callback Settings", CenterLabel = true)]
        [LabelWidth(100)]
        public UnityEvent[] OnSoundFinished;

        private static IEnumerable<TriggerType> TriggerTypeOptions => Enum.GetValues(typeof(TriggerType)) as TriggerType[];

        public enum TriggerType
        {
            Collider,
            TimeThreshold,
            CustomEvent
        }

        private bool IsColliderTrigger => Trigger == TriggerType.Collider;
        private bool IsTimeTrigger => Trigger == TriggerType.TimeThreshold;
        private bool IsCustomEventTrigger => Trigger == TriggerType.CustomEvent;
    }

    [FoldoutGroup("Story Sounds", true)]
    [TableList(AlwaysExpanded = true)]
    public List<StorySound> StorySounds = new List<StorySound>();

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        foreach (var sound in StorySounds)
        {
            if (sound.Triggered) continue;

            switch (sound.Trigger)
            {
                case StorySound.TriggerType.Collider:
                    CheckColliderTrigger(sound);
                    break;
                case StorySound.TriggerType.TimeThreshold:
                    if (_elapsedTime >= sound.TriggerTime)
                    {
                        PlaySound(sound);
                        sound.Triggered = true;
                    }
                    break;
                case StorySound.TriggerType.CustomEvent:
                    // Custom events need to be triggered externally
                    break;
            }
        }
    }

    private void CheckColliderTrigger(StorySound sound)
    {
        if (sound.TriggerObject == null || !sound.TriggerObject.TryGetComponent(out Collider2D collider)) return;

        if (collider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            PlaySound(sound);
            sound.Triggered = true;
        }
    }

    public void TriggerCustomEvent(string eventName)
    {
        foreach (var sound in StorySounds)
        {
            if (sound.Trigger == StorySound.TriggerType.CustomEvent && sound.CustomEventName == eventName)
            {
                PlaySound(sound);
                sound.Triggered = true;
            }
        }
    }

    private void PlaySound(StorySound sound)
    {
        AudioSource audioSource = SoundManager.Instance.PlaySoundClip(
            sound.SoundClip,
            PlayerMovement.Instance.transform,
            SoundManager.SoundType.LOUD_FX,
            SoundManager.SoundFXType.AMBIENT,
            followTarget: sound.PropagationSource.transform);

        StartCoroutine(WaitForSoundToFinish(audioSource, sound.OnSoundFinished));

        if (sound.PropagateSound)
            SoundPropagationManager.Instance.PropagateSound(sound.PropagationSource.transform.position, SoundOrigin.PLAYER, 0.5f);
    }

    private IEnumerator WaitForSoundToFinish(AudioSource audioSource, UnityEvent[] callbacks)
    {
        yield return new WaitWhile(() => audioSource != null && audioSource.isPlaying);

        if (callbacks != null)
        {
            foreach (var callback in callbacks)
            {
                callback.Invoke();
            }
        }
    }
}
