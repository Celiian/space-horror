using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class GlitchZone : MonoBehaviour
{

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("Audio clip to play on glitch activation.")]
    private AudioClip audioClip;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("Type of sound to be used.")]
    private SoundManager.SoundType soundType;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("GameObject that emits the sound.")]
    private GameObject soundEmitter;

    [FoldoutGroup("Sound Propagation")]
    [SerializeField, Tooltip("Attenuation factor for the sound volume.")]
    private float volumeAttenuation;
    private bool isGlitching = false;

    private AudioSource audioSource;

    private DG.Tweening.Sequence glitchSequence;


    private void Update() {
        if (isGlitching && (glitchSequence == null || !glitchSequence.IsActive() || glitchSequence.IsComplete())) {
            float randomNoiseAmount = Random.Range(10, 100); // Random value between 50 and 150
            float randomGlitchStrength = Random.Range(10, 100); // Random value between 50 and 150
            float randomScanLinesStrength = Random.Range(0.1f, 1f); // Random value between 0 and 50

            glitchSequence = DOTween.Sequence().Append(DOTween.To(() => GlitchManager.Instance.noiseAmount, x => GlitchManager.Instance.noiseAmount = x, randomNoiseAmount, 0.1f)).Append(DOTween.To(() => GlitchManager.Instance.glitchStrength, x => GlitchManager.Instance.glitchStrength = x, randomGlitchStrength, 0.1f)).Append(DOTween.To(() => GlitchManager.Instance.scanLinesStrength, x => GlitchManager.Instance.scanLinesStrength = x, randomScanLinesStrength, 0.1f));

            audioSource.transform.position = PlayerMovement.Instance.transform.position;
        }


    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            isGlitching = true;
            if(audioClip != null)
            {
                audioSource = SoundManager.Instance.PlaySoundClip(audioClip, PlayerMovement.Instance.transform, soundType, SoundManager.SoundFXType.AMBIENT, looped: true, followTarget: PlayerMovement.Instance.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            isGlitching = false;
            if(audioSource != null) {
                audioSource.Stop();
                Destroy(audioSource.gameObject);
                audioSource = null;
            }

            if(glitchSequence != null) {
                glitchSequence.Kill();
                glitchSequence = null;
                GlitchManager.Instance.SetGlitch(0, 0, 1);
            }
        }
    }
}
