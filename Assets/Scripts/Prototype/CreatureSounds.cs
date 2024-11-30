using UnityEngine;
using static ManageLightAndSound;

public class CreatureSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] leftFootstepSounds;
    [SerializeField] private AudioClip[] rightFootstepSounds;

    private bool isLeftFoot = true;

    public void PlayFootstepSound(ManageLightAndSoundSettings manageLightAndSoundSettings)
    {
        AudioClip[] footstepSounds = isLeftFoot ? leftFootstepSounds : rightFootstepSounds;
        SoundManager.Instance.PlayRandomSoundClip(footstepSounds, transform, SoundManager.SoundType.FOOTSTEPS, SoundManager.SoundFXType.FX, manageLightAndSoundSettings);
        isLeftFoot = !isLeftFoot;
    }
}
