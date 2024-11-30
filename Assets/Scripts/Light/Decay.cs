using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Decay : MonoBehaviour
{
    private Light2D light;
    private float initialIntensity;
    private float duration = 2f;

    private void Start()
    {
        light = GetComponent<Light2D>();
        initialIntensity = light.intensity;
    }

    private void Update()
    {
        // DOTWEEN with easing
        DOTween.To(() => light.intensity, x => light.intensity = x, 0f, duration)
               .SetEase(Ease.InExpo);
    }
}
