using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StorySoundManager : MonoBehaviour
{
    [Serializable]
    public class StorySound
    {
        [BoxGroup("General Settings", CenterLabel = true)]
        [LabelWidth(100)]
        public AudioClip SoundClip;

        [BoxGroup("Trigger Settings", CenterLabel = true)]
        [LabelWidth(100)]
        [LabelText("Trigger Type")]
        [ValueDropdown(nameof(TriggerTypeOptions))]
        public TriggerType Trigger;

        [BoxGroup("Trigger Settings")]
        [ShowIf(nameof(IsColliderTrigger))]
        [Tooltip("Specify a GameObject with a collider.")]
        public GameObject TriggerObject;

        [BoxGroup("Trigger Settings")]
        [ShowIf(nameof(IsTimeTrigger))]
        [Tooltip("Time in seconds after the scene starts.")]
        [LabelWidth(100)]
        public float TriggerTime;

        [BoxGroup("Trigger Settings")]
        [ShowIf(nameof(IsCustomEventTrigger))]
        [Tooltip("Event to trigger this sound.")]
        [LabelWidth(120)]
        public string CustomEventName;

        [HideInInspector]
        public bool Triggered;

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
        SoundManager.Instance.PlaySoundClip(
            sound.SoundClip,
            PlayerMovement.Instance.transform,
            SoundManager.SoundType.LOUD_FX,
            SoundManager.SoundFXType.AMBIENT,
            followPlayer:true);
    }
}
