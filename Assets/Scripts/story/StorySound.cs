using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using TMPro;

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
        public SoundManager.SoundType SoundType;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public bool PropagateSound;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public GameObject PropagationSource;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public SoundOrigin SoundOrigin;

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        public float Attenuation;

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

        [VerticalGroup("Settings")]
        [BoxGroup("Settings/General Settings")]
        [LabelWidth(100)]
        [Tooltip("Subtitle text for the sound.")]
        public string SubtitleText;

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

    [FoldoutGroup("Subtitle Display", true)]
    [SerializeField]
    [Tooltip("UI Text element to display subtitles.")]
    private TextMeshProUGUI subtitleTextUI;

    [FoldoutGroup("Subtitle Display")]
    [SerializeField]
    [Tooltip("Canvas to display subtitles on.")]
    private Canvas _subtitleCanvas;

    private float _elapsedTime;

    private int _subtitlesDisplayed = 0;

    private void Start()
    {
        _subtitleCanvas.enabled = false;
    }

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
            }
        }
    }

    public void TriggerCustomEvent(string eventName)
    {
        Debug.Log("TriggerCustomEvent: " + eventName);
        foreach (var sound in StorySounds)
        {
            if (sound.Trigger == StorySound.TriggerType.CustomEvent && sound.CustomEventName == eventName)
            {
                PlaySound(sound);
                sound.Triggered = true;
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

    
    private void PlaySound(StorySound sound)
    {
        // Display the subtitle
        if (subtitleTextUI != null)
        {
            _subtitleCanvas.enabled = true;
            subtitleTextUI.text = sound.SubtitleText;

            _subtitlesDisplayed++;
        }

        AudioSource audioSource = SoundManager.Instance.PlaySoundClip(
            sound.SoundClip,
            PlayerMovement.Instance.transform,
            sound.SoundType,
            SoundManager.SoundFXType.FX,
            followTarget: sound.PropagationSource.transform);

        StartCoroutine(WaitForSoundToFinish(audioSource, sound.OnSoundFinished));

        if (sound.PropagateSound)
            SoundPropagationManager.Instance.PropagateSound(sound.PropagationSource.transform.position, sound.SoundOrigin, sound.Attenuation);
    }

    private IEnumerator WaitForSoundToFinish(AudioSource audioSource, UnityEvent[] callbacks)
    {
        yield return new WaitWhile(() => audioSource != null && audioSource.isPlaying);

        _subtitlesDisplayed--;
        if (_subtitlesDisplayed == 0)
        {
            _subtitleCanvas.enabled = false;
            subtitleTextUI.text = "";
        }

        if (callbacks != null)
        {
            foreach (var callback in callbacks)
            {
                callback.Invoke();
            }
        }
    }
}
