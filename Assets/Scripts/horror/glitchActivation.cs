using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using Sirenix.Serialization;

public class GlitchActivation : MonoBehaviour
{
    [FoldoutGroup("Glitch Settings")]
    [SerializeField, Tooltip("Duration of the glitch effect in seconds.")]
    private float duration;

    [FoldoutGroup("Glitch Settings")]
    [SerializeField, Tooltip("If true, while the player is in the trigger, the glitch will be active.")]
    private bool isPermanent;

    [FoldoutGroup("Glitch Settings")]
    [SerializeField, Tooltip("Determines if the glitch activation should occur only once.")]
    private bool isOneTime;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("Audio clip to play on glitch activation.")]
    private AudioClip audioClip;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("Type of sound to be used.")]
    private SoundManager.SoundType soundType;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("If true, the sound will be propagated.")]
    private bool propagateSound = true;

    [FoldoutGroup("Audio Settings")]
    [SerializeField, Tooltip("GameObject that emits the sound.")]
    private GameObject soundEmitter;

    [FoldoutGroup("Sound Propagation")]
    [SerializeField, Tooltip("Attenuation factor for the sound volume.")]
    private float volumeAttenuation;

    private Coroutine _glitchCoroutine;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            if(isPermanent)
            {
                _glitchCoroutine = StartCoroutine(GlitchCoroutine());
            }
            else
            {
                GlitchManager.Instance.Glitch(duration);
            }
            
            if(audioClip != null)
            {
                SoundManager.Instance.PlaySoundClip(audioClip, soundEmitter.transform, soundType, SoundManager.SoundFXType.FX);
                if(propagateSound)
                {
                    SoundPropagationManager.Instance.PropagateSound(soundEmitter.transform.position, SoundOrigin.ITEM, volumeAttenuation);
                }
            }

            if(isOneTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            if(isPermanent && _glitchCoroutine != null)
            {
                StopAllCoroutines();
                GlitchManager.Instance.SetGlitch(0, 0, 0.6f);
                _glitchCoroutine = null;
            }
        }
    }

    private IEnumerator GlitchCoroutine()
    {
        while (true){
            GlitchManager.Instance.Glitch(duration);
            yield return new WaitForSeconds(duration);
        }
    }
}
