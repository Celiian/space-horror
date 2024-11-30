using System.Collections;
using UnityEngine;

public class ManageLight : MonoBehaviour
{
    public void Activate(ManageLightData manageLightData)
    {
        StartCoroutine(LightCoroutine(manageLightData));
    }

    private IEnumerator LightCoroutine(ManageLightData manageLightData)
    {
        while (true)
        {
            float distance = CalcUtils.DistanceToTarget(GameManager.Instance.player.transform, transform);
            if(distance < manageLightData.soundPropagationDistance){
                ParticleSystem particle = manageLightData.lightParticleSystem.GetComponent<ParticleSystem>();
                var shape = particle.shape;
                var main = particle.main;
                main.startLifetime = manageLightData.particleLifeTime;
                shape.radius = manageLightData.radius;
                particle.emission.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0f, manageLightData.particleCount)
                });
                AttachGameObjectsToParticles lightParticle = Instantiate(manageLightData.lightParticleSystem, transform.position, Quaternion.identity).GetComponent<AttachGameObjectsToParticles>();
                lightParticle.color = manageLightData.color;
            }
            yield return new WaitForSeconds(manageLightData.timeBetweenPulses);
        }
    }                           
}
[System.Serializable]
    public class ManageLightData {
    public float soundPropagationDistance = 1f;
    public GameObject lightParticleSystem;
    public Color color = Color.white;
    public float particleCount = 60;
    public float particleLifeTime = 2f;
    public float radius = 1f;
    public float timeBetweenPulses = 0.5f;
}