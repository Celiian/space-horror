using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using static SoundManager;
using static StorySoundManager;
using System.Collections;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
public class IntroManager : MonoBehaviour {
    public static IntroManager Instance;
    [FoldoutGroup("References")]
    [SerializeField] private StorySoundManager storySoundManager;
    [FoldoutGroup("References")]
    [SerializeField] private FollowTarget followTarget;
    [FoldoutGroup("References")]
    [SerializeField] private GameObject station;


    [FoldoutGroup("Settings")]
    [SerializeField]
    [Tooltip("Position to teleport the player to.")]
    private GameObject playerTeleportPosition;

    [FoldoutGroup("Events")]
    [SerializeField] private UnityEvent _onSkipIntro;

    [FoldoutGroup("Subtitle Display", true)]
    [SerializeField]
    [Tooltip("UI Text element to display subtitles.")]
    private TextMeshProUGUI subtitleTextUI;

    [FoldoutGroup("Subtitle Display")]
    [SerializeField]
    [Tooltip("Canvas to display subtitles on.")]
    private Canvas _subtitleCanvas;
    
    
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioSource ambianceAudioSource;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioSource sawAudioSource;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip yourBrainRequires;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip youAreNowBlind;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip sawStarting;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip sawPlaying;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip sawEnding;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip violin;
    [FoldoutGroup("Sounds")]
    [SerializeField] private AudioClip teleportSound;


    private int _subtitlesDisplayed = 0;
    private List<Entity> entitiesToUnpause = new List<Entity>();
    private Sequence cameraSequence;
    private bool _isRoomIlluminated = true;
    private Coroutine introCoroutine;

    public bool skipIntro = false;

    private void Awake() {
        Instance = this;
        station.SetActive(true);
    }

    private void Start() {
        ambianceAudioSource.Pause();
        Entity[] entities =  FindObjectsOfType<Entity>();
        foreach (var entity in entities) {
            if(!entity.TryGetComponent<PlayerMovement>(out _) && !entity.isPaused){
                entitiesToUnpause.Add(entity);
                entity.isPaused = true;
            }
        }
        storySoundManager.isPaused = true;
        PlayerMovement.Instance.isPaused = true;
        introCoroutine = StartCoroutine(StartIntro());
    }

    public void Update(){
        if(skipIntro){
            SkipIntro();
        }
    }

    public void SkipIntro(){
        _isRoomIlluminated = false;
        StopAllCoroutines();
        cameraSequence.Kill();
        station.SetActive(false);
        Camera.main.orthographicSize = 7;
        PlayerMovement.Instance.transform.position = playerTeleportPosition.transform.position;
        followTarget.CenterOnTarget();
        storySoundManager.isPaused = false;
        storySoundManager._elapsedTime = 30;
        PlayerMovement.Instance.isPaused = false;
        UnpauseEntities();
        ambianceAudioSource.Play();
    }


    public IEnumerator StartIntro() {
        cameraSequence = DOTween.Sequence();
        cameraSequence.Append(DOTween.To(() => 4000, (x) => Camera.main.orthographicSize = x, 7, 10f));
        yield return cameraSequence.WaitForCompletion();
        SoundPropagationManager.Instance.Init();
        PlayerMovement.Instance.gameObject.GetComponent<PlayerDangerDetector>().dangerIndicator.SetActive(true);

        StartCoroutine(illuminateRoom());
        PlaySound(yourBrainRequires, "   Your brain require some energy. I will now procede to remove your eyes.");
        yield return new WaitForSeconds(yourBrainRequires.length);
        PlaySound(violin, "", 0.2f);
        sawAudioSource.clip = sawStarting;
        sawAudioSource.Play();
        yield return new WaitForSeconds(sawStarting.length);
        sawAudioSource.Stop();
        sawAudioSource.clip = sawPlaying;
        sawAudioSource.Play();
        yield return new WaitForSeconds(violin.length - sawStarting.length - sawEnding.length);
        sawAudioSource.Stop();
        sawAudioSource.clip = sawEnding;
        sawAudioSource.Play();
        yield return new WaitForSeconds(sawEnding.length);
        PlaySound(youAreNowBlind, "You are now blind, but don't worry—I've enhanced your hearing and provided a visualization to help.");
        _isRoomIlluminated = false;
        yield return new WaitForSeconds(youAreNowBlind.length);
        PlayerMovement.Instance.isPaused = false;
    }

    private IEnumerator illuminateRoom() {
        while (_isRoomIlluminated) {
            yield return new WaitForSeconds(0.1f);
            SoundPropagationManager.Instance.PropagateSound(PlayerMovement.Instance.transform.position, SoundOrigin.PLAYER, 10);
        }


    }

    public void TeleportPlayer() {
        StartCoroutine(TeleportPlayerCoroutine());
    }


    private IEnumerator TeleportPlayerCoroutine() {
        station.SetActive(false);
        _isRoomIlluminated = true;
        PlayerMovement.Instance.isPaused = true;
        yield return new WaitForSeconds(0.6f);
        PlaySound(teleportSound, "");
        Material playerMAT = PlayerMovement.Instance.gameObject.GetComponent<SpriteRenderer>().material;
        Sequence teleportSequence = DOTween.Sequence();
        teleportSequence.Append(DOTween.To(() => 0, (x) => playerMAT.SetFloat("_DissolveAmount", x), 1.1f, teleportSound.length));
        yield return teleportSequence.WaitForCompletion();
        PlayerMovement.Instance.transform.position = playerTeleportPosition.transform.position;
        followTarget.CenterOnTarget();
        storySoundManager.isPaused = false;
        yield return new WaitForSeconds(0.2f);
        teleportSequence = DOTween.Sequence();
        teleportSequence.Append(DOTween.To(() => 1.1f, (x) => playerMAT.SetFloat("_DissolveAmount", x), 0f, 6f));
        yield return new WaitForSeconds(0.5f);
        ambianceAudioSource.Play();
        _isRoomIlluminated = false;
    }

    public void UnpauseEntities() {
        foreach (var entity in entitiesToUnpause) {
            entity.isPaused = false;
        }
    }


    private void PlaySound(AudioClip sound, string subtitleText, float attenuation = 1f)
    {
        // Display the subtitle
        if (subtitleTextUI != null)
        {
            _subtitleCanvas.enabled = true;
            subtitleTextUI.text = subtitleText;

            _subtitlesDisplayed++;
        }

        AudioSource audioSource = SoundManager.Instance.PlaySoundClip(
            sound,
            PlayerMovement.Instance.transform,
            SoundType.FX,
            SoundFXType.FX,
            followTarget: PlayerMovement.Instance.transform,
            additionalAttenuation: attenuation);

        StartCoroutine(WaitForSoundToFinish(audioSource));


        SoundPropagationManager.Instance.PropagateSound(PlayerMovement.Instance.transform.position, SoundOrigin.PLAYER, 1);
    }

    private IEnumerator WaitForSoundToFinish(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource != null && audioSource.isPlaying);

        _subtitlesDisplayed--;
        if (_subtitlesDisplayed == 0)
        {
            _subtitleCanvas.enabled = false;
            subtitleTextUI.text = "";
        }
    }
}
