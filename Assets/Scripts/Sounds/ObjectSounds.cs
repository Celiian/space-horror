using System.Collections;
using UnityEngine;
using static ManageLightAndSound;

public class ObjectSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private SoundManager.SoundType volume;
    [SerializeField] private ManageLightAndSoundSettings manageLightAndSoundSettings;

    private void Start()
    {
        StartCoroutine(PlaySoundCoroutine());
    }

    private IEnumerator PlaySoundCoroutine()
    {
        while(true)
        {
            SoundManager.Instance.PlaySoundClip(clip, transform, volume, SoundManager.SoundFXType.FX, manageLightAndSoundSettings);
            yield return new WaitForSeconds(clip.length);
        }
    }
}
