using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using DG.Tweening;

public class LightOnSound : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private CreatureSounds creatureSounds;
    [SerializeField] private GameObject particlePrefab;
    public float speed;
    private Rigidbody2D rb;
    
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(UpdateLightCoroutine());
    }

    private IEnumerator UpdateLightCoroutine() {
        while (true) {
            UpdateLightOnMovement();
            yield return new WaitForSeconds(0.5f * speed);
        }
    }

    private void UpdateLightOnMovement(){
        float lightIntensityFactor = rb.velocity.magnitude / 12;
        float targetRadius = lightIntensityFactor * 10;
        if(Mathf.Max(targetRadius, 2f) <= 2f){
            return;
        }

        ParticleSystem particle = particlePrefab.GetComponent<ParticleSystem>();
        particle.emission.SetBursts(new ParticleSystem.Burst[] {
             new ParticleSystem.Burst(0f, 50)
        });

        Instantiate(particlePrefab, transform.position, Quaternion.identity);

        // float targetIntensity = rb.velocity.magnitude / 12;
        // float targetRadius = rb.velocity.magnitude * 6;
        // _light.pointLightOuterRadius = Mathf.Max(targetRadius * 0.6f, 2f);
        // _light.intensity = Mathf.Max(targetIntensity * 0.6f, 0.1f);
        // if(_light.pointLightOuterRadius <= 2f){
        //     return;
        // }
        // creatureSounds.PlayFootstepSound();
        // DOTween.To(() => _light.intensity, 
        //            x => _light.intensity = x, 
        //            targetIntensity * 0.6f, 
        //            0.5f * speed);
    }
}
