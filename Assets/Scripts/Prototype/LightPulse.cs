using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulse : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Player player;
    [SerializeField] private float distanceThreshold = 10f;

    private float baseIntensity;
    private float baseRadius;

    void Start()
    {
        StartCoroutine(PulseLight());
    }


    private IEnumerator PulseLight()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.4f);
            if(DistanceToTarget() > distanceThreshold) {
                continue;
            }
            ParticleSystem particle = particlePrefab.GetComponent<ParticleSystem>();
            particle.emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 60)
            });
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
    }

    private float DistanceToTarget()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }
}
