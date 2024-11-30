using UnityEngine;
using System.Collections;
using static ManageLightAndSound;
using Sirenix.OdinInspector;

public class FootSteps : MonoBehaviour
{
    [SerializeField] public ManageLightAndSoundSettings manageLightAndSoundSettings;
    [SerializeField] private CreatureSounds creatureSounds;
    public float speed = 1f;

    private Coroutine playFootstepSoundCoroutine;
    
    private void Start()
    {
        StartCoroutine(PlayFootstepSoundCoroutine());
    }
    private bool IsMoving() => manageLightAndSoundSettings.rb.velocity.magnitude > 0.1f;

    private IEnumerator PlayFootstepSoundCoroutine()
    { 
        while (true)
        {
            float speedFactor = manageLightAndSoundSettings.rb.velocity.magnitude;
            float intervalModifier;

            if (speedFactor - 0.1f > manageLightAndSoundSettings.maxSpeed)
            {
                intervalModifier = 0.6f;
            }
            else if (speedFactor + 0.1f < manageLightAndSoundSettings.maxSpeed)
            {
                intervalModifier = 1.3f;
            }
            else
            {
                intervalModifier = 1f;
            }

            float interval = 0.5f * intervalModifier;
            yield return new WaitForSeconds(interval);
            if (IsMoving())
                creatureSounds.PlayFootstepSound(manageLightAndSoundSettings);
        }
    }
}
