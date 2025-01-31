using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using static SoundManager;
using static StorySoundManager;
using System.Collections;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class IntroManager : MonoBehaviour {
    public static IntroManager Instance;
    [FoldoutGroup("References")]
    [SerializeField] private StorySoundManager storySoundManager;
    [FoldoutGroup("References")]
    [SerializeField] private FollowTarget followTarget;
    [FoldoutGroup("References")]
    [SerializeField] private GameObject station;
    [FoldoutGroup("References")]
    [SerializeField] private GameObject keyboardInput;
    [FoldoutGroup("References")]
    [SerializeField] private Volume volume;
    [FoldoutGroup("References")]
    [SerializeField] private GameObject objectiveUi;
    [FoldoutGroup("References")]
    [SerializeField] private GameObject objectiveIndicator;



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

    private List<AudioSource> introSources = new List<AudioSource>();


    private int _subtitlesDisplayed = 0;
    private List<Entity> entitiesToUnpause = new List<Entity>();
    private Sequence cameraSequence;
    private bool _isRoomIlluminated = true;
    private Coroutine introCoroutine;
    public bool skipIntro = false;
    private bool _isKeyboardInputShown = false;
    public bool isInIntro = true;

    private Sequence teleportSequence;
    private Sequence dissolveSequence;
    private Sequence saturationSequence;

    [Button]
    public void TestTween(float amplitude, float period) {
        volume.profile.TryGet(out LensDistortion lensDistortion);
        lensDistortion.intensity.value = 0.8f;
        DOTween.To(() => lensDistortion.intensity.value, 
        (x) => lensDistortion.intensity.value = x, 
        0, 
        1.5f).SetEase(Ease.OutElastic, amplitude, period);
    }

    private void Awake() {
        Instance = this;
        keyboardInput.SetActive(false);
        station.SetActive(true);
        objectiveUi.SetActive(false);
        objectiveIndicator.SetActive(false);
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

        if(_isKeyboardInputShown){
            if(PlayerMovement.Instance.isMoving){
                _isKeyboardInputShown = false;
                keyboardInput.SetActive(false);
            }
        }
    }

    public void SkipIntro(){
        isInIntro = false;
        // Show the objective ui
        objectiveUi.SetActive(true);
        objectiveIndicator.SetActive(false);
        // Set the saturation to -80 to simulate the player being blind
        volume.profile.TryGet(out ColorAdjustments colorAdjustments);
        colorAdjustments.saturation.value = -80;
        
        // Init the sound propagation manager
        SoundPropagationManager.Instance.Init();

        // Stop all coroutines
        StopAllCoroutines();

        // Disable the room illumination
        _isRoomIlluminated = false;

        // Set the story sound manager elapsed time
        storySoundManager._elapsedTime = 25;

        // Kill the camera sequence
        cameraSequence.Kill();

        // Disable the station
        station.SetActive(false);

        // Teleport the player to the teleportation room
        PlayerMovement.Instance.transform.position = playerTeleportPosition.transform.position;
        followTarget.CenterOnTarget();
        Camera.main.orthographicSize = 7;

        // Unpause the player and show the keyboard input tips
        PlayerMovement.Instance.isPaused = false;
        ShowKeyboardInput();

        // Unpause the entities
        UnpauseEntities();

        // Play the ambiance sound and unpause the story sound manager
        ambianceAudioSource.Play();
        storySoundManager.isPaused = false;

        // Stop the intro sounds
        foreach(var source in introSources){
            source.Stop();
        }
        sawAudioSource.Stop();

        // Skip the objectives
        ObjectivesManager.Instance.SkipCurrentObjective();

    }


    public IEnumerator StartIntro() {
        // Init Intro variables and managers
        SoundPropagationManager.Instance.Init();
        volume.profile.TryGet(out ColorAdjustments colorAdjustments);
        colorAdjustments.saturation.value = 0;
        float saturationDuration = sawStarting.length + violin.length + sawEnding.length;

        // Camera sequence to zoom in. At the beginning we see a planet and the station, and at the end the teleportation room from the player ship only.
        cameraSequence = DOTween.Sequence();
        cameraSequence.Append(DOTween.To(() => 4000, (x) => Camera.main.orthographicSize = x, 7, 10f));

        // Illuminate the room because the player is not blind yet
        StartCoroutine(illuminateRoom());

        // Lore sound
        introSources.Add(PlaySound(yourBrainRequires, "Your brain require some energy. I will now procede to remove your eyes.", 0.6f));

        yield return new WaitForSeconds(yourBrainRequires.length);
        
        // Change the saturation gradually to simulate the player's vision being removed
        saturationSequence = DOTween.Sequence();
        saturationSequence.Append(DOTween.To(() => 0, (x) => colorAdjustments.saturation.value = x, -80, saturationDuration));
        saturationSequence.Play();

        // Play the violin and saw sounds
        introSources.Add(PlaySound(violin, "", 0.1f));
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

        // Lore explanation of why the player can still "see"
        introSources.Add(PlaySound(youAreNowBlind, "You are now blind, but don't worry—I've enhanced your hearing and provided a visualization to help.", 0.6f));

        // Disable the room illumination because the player is blind now
        _isRoomIlluminated = false;

        yield return new WaitForSeconds(youAreNowBlind.length);

        // Unpause the player and show the keyboard input tips
        PlayerMovement.Instance.isPaused = false;
        objectiveUi.SetActive(true);
        objectiveIndicator.SetActive(true);
        ShowKeyboardInput();
        isInIntro = false;
    }

    private void ShowKeyboardInput(){
        _isKeyboardInputShown = true;
        keyboardInput.SetActive(true);
    }

    private IEnumerator illuminateRoom() {
        while (_isRoomIlluminated) {
            yield return new WaitForSeconds(0.1f);
            SoundPropagationManager.Instance.PropagateSound(PlayerMovement.Instance.transform.position, SoundOrigin.PLAYER, 4);
        }
    }

    public void TeleportPlayer() {
        StartCoroutine(TeleportPlayerCoroutine());
    }


    private IEnumerator TeleportPlayerCoroutine() {
        isInIntro = true;
        objectiveUi.SetActive(false);
        objectiveIndicator.SetActive(false);
        // Init the teleportation variables
        Material playerMAT = PlayerMovement.Instance.gameObject.GetComponent<SpriteRenderer>().material;
        volume.profile.TryGet(out LensDistortion lensDistortion);
        lensDistortion.intensity.value = 0;

        // Disable the station because it is hiding the map
        station.SetActive(false);

        // Illuminate the room because of the teleportation sounds
        _isRoomIlluminated = true;

        // Pause the player movements
        PlayerMovement.Instance.isPaused = true;

        yield return new WaitForSeconds(0.6f);

        // Play the teleportation sound
        introSources.Add(PlaySound(teleportSound, ""));

        // Dissolve the player and add some lens distortion and screenshake
        dissolveSequence = DOTween.Sequence();
        dissolveSequence.Append(DOTween.To(() => 0, (x) => playerMAT.SetFloat("_DissolveAmount", x), 1.1f, teleportSound.length));
        dissolveSequence.Join(DOTween.To(() => 0, (x) => lensDistortion.intensity.value = x, 0.8f, teleportSound.length));
        CameraShake.instance.ShakeAdditive(teleportSound.length, 0.4f);
        dissolveSequence.Play();

        yield return dissolveSequence.WaitForCompletion();

        // Teleport the player to the teleportation room
        PlayerMovement.Instance.transform.position = playerTeleportPosition.transform.position;
        followTarget.CenterOnTarget();

        // Remove the lens distortion and screenshake gradually
        CreateLensDistortionSequence(lensDistortion);

        CreateDissolveSequence(playerMAT, 0f);
        CameraShake.instance.ShakeSubtractive(teleportSequence.Duration(), 2f);
        dissolveSequence.Play();
        teleportSequence.Play();

        // Unpause the story sound manager to continue the intro
        storySoundManager.isPaused = false;

        yield return new WaitForSeconds(0.2f);

        // Play the ambiance sound and disable the room illumination because the loud sounds are over
        ambianceAudioSource.Play();
        _isRoomIlluminated = false;
        objectiveUi.SetActive(true);
        objectiveIndicator.SetActive(true);
        isInIntro = false;
    }

    // Create a sequence to gradually remove the lens distortion
    private void CreateLensDistortionSequence(LensDistortion lensDistortion) {
        teleportSequence = DOTween.Sequence();
        float[] values = { -0.5f, 0.4f, -0.3f, 0.2f, 0f };
        float duration = 0.3f;

        foreach (var value in values) {
            teleportSequence.Append(DOTween.To(() => lensDistortion.intensity.value, 
                                                (x) => lensDistortion.intensity.value = x, 
                                                value, 
                                                duration));
        }
    }

    // Create a sequence to dissolve the player
    private void CreateDissolveSequence(Material playerMAT, float endValue) {
        dissolveSequence = DOTween.Sequence();
        dissolveSequence.Append(DOTween.To(() => 1.1f, 
                                           (x) => playerMAT.SetFloat("_DissolveAmount", x), 
                                           endValue, 
                                           16f));
    }

    public void UnpauseEntities() {
        foreach (var entity in entitiesToUnpause) {
            entity.isPaused = false;
        }
    }


    private AudioSource PlaySound(AudioClip sound, string subtitleText, float attenuation = 1f)
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
        return audioSource;
    }

    private IEnumerator WaitForSoundToFinish(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource != null && audioSource.isPlaying);
        yield return new WaitForSeconds(0.5f);

        _subtitlesDisplayed--;
        if (_subtitlesDisplayed == 0)
        {
            _subtitleCanvas.enabled = false;
            subtitleTextUI.text = "";
        }
    }
}
