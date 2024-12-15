using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

public class GlitchActivation : MonoBehaviour
{
    [FoldoutGroup("Glitch Settings")]
    [SerializeField, Tooltip("Duration of the glitch effect in seconds.")]
    private float duration;

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
    [SerializeField, Tooltip("GameObject that emits the sound.")]
    private GameObject soundEmitter;

    [FoldoutGroup("Sound Propagation")]
    [SerializeField, Tooltip("Attenuation factor for the sound volume.")]
    private float volumeAttenuation;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            GlitchManager.Instance.Glitch(duration);
            
            if(audioClip != null)
            {
                SoundManager.Instance.PlaySoundClip(audioClip, soundEmitter.transform, soundType, SoundManager.SoundFXType.FX);
                SoundPropagationManager.Instance.PropagateSound(soundEmitter.transform.position, SoundOrigin.ITEM, volumeAttenuation);
            }

            if(isOneTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
