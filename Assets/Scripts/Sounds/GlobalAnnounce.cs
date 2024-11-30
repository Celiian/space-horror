using System.Collections;
using UnityEngine;
using static ManageLightAndSound;

public class GlobalAnnounce : MonoBehaviour
{
    [SerializeField] private AudioClip alertBeep;
    [SerializeField] private AudioClip alertVoice;
    [SerializeField] private float interval = 10f;
    [SerializeField] private ManageLightAndSoundSettings manageLightAndSoundSettingsBeep;
    [SerializeField] private ManageLightAndSoundSettings manageLightAndSoundSettingsVoice;
    public bool continuePlaying = true;

    private void Start()
    {
        StartCoroutine(PlayAlertBeepAndVoice());
    }
    
    IEnumerator PlayAlertBeepAndVoice(){
        yield return new WaitForSeconds(2f);
        while(continuePlaying){
            SoundManager.Instance.PlaySoundClip(alertBeep, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX, manageLightAndSoundSettingsBeep);
            yield return new WaitForSeconds(alertBeep.length);
            SoundManager.Instance.PlaySoundClip(alertVoice, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX, manageLightAndSoundSettingsVoice);
            yield return new WaitForSeconds(interval);
        }
    }

}
