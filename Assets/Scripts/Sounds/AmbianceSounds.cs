using System.Collections;
using UnityEngine;

public class AmbianceSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float minInterval;
    [SerializeField] private float maxInterval;


    void Start()
    {
        StartCoroutine(PlayRandomClip());
    }
    
    private IEnumerator PlayRandomClip(){
        while(true){
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            Debug.Log(clip.name);
            SoundManager.Instance.PlaySoundClip(clip, transform, SoundManager.SoundType.AMBIENT, SoundManager.SoundFXType.AMBIENT);
        }
    }
}
