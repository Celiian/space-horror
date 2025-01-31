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
        public bool PropagateOnce;

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
        [ShowIf(nameof(IsEnemyTrigger))]
        [Tooltip("Specify a GameObject with a collider.")]
        public GameObject EnemyTriggerObject;

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
            EnemyTrigger,
            Collider,
            TimeThreshold,
            CustomEvent,
        }

        private bool IsEnemyTrigger => Trigger == TriggerType.EnemyTrigger;
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

    public float _elapsedTime;

    private int _subtitlesDisplayed = 0;

    public bool isPaused = false;

    private const float TimeThreshold = 0.5f; // Define a small threshold value
    
    private List<AudioSource> audioSources = new List<AudioSource>();

    private Coroutine _illuminateRoomCoroutine;

    private void Start()
    {
        _subtitleCanvas.enabled = false;
        _illuminateRoomCoroutine = StartCoroutine(illuminateRoom());
    }

    private void Update()
    {
        if(isPaused) return;
        _elapsedTime += Time.deltaTime;

        foreach (var sound in StorySounds)
        {
            if (sound.Triggered) continue;

            switch (sound.Trigger)
            {
                case StorySound.TriggerType.EnemyTrigger:
                    CheckEnemyTrigger(sound);
                    break;
                case StorySound.TriggerType.Collider:
                    CheckColliderTrigger(sound);
                    break;
                case StorySound.TriggerType.TimeThreshold:
                    if (Mathf.Abs(_elapsedTime - sound.TriggerTime) <= TimeThreshold)
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
        foreach (var sound in StorySounds)
        {
            if (sound.Trigger == StorySound.TriggerType.CustomEvent && sound.CustomEventName == eventName)
            {
                PlaySound(sound);
                sound.Triggered = true;
            }
        }
    }

    private void CheckEnemyTrigger(StorySound sound)
    {
         if (sound.EnemyTriggerObject == null || !sound.EnemyTriggerObject.TryGetComponent(out Collider2D collider)) return;

        if (collider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            PlaySound(sound);
            sound.Triggered = true;
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
            if(!sound.PropagateOnce) {
                audioSources.Add(audioSource);
                if(_illuminateRoomCoroutine == null) {
                    _illuminateRoomCoroutine = StartCoroutine(illuminateRoom());
                }
            }
            else
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

        if(audioSources.Contains(audioSource)) {
            audioSources.Remove(audioSource);
            if(audioSources.Count == 0) {
                StopCoroutine(_illuminateRoomCoroutine);
                _illuminateRoomCoroutine = null;
            }
        }

        if (callbacks != null)
        {
            foreach (var callback in callbacks)
            {
                callback.Invoke();
            }
        }
    }

    public void ResetTime() {
        _elapsedTime = 0f;
    }


    private IEnumerator illuminateRoom() {
        while (true){
            yield return new WaitForSeconds(0.4f);
            SoundPropagationManager.Instance.PropagateSound(PlayerMovement.Instance.transform.position, SoundOrigin.PLAYER, 1);
        }
    }
}
