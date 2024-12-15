using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public static GlitchManager Instance;

    private void Awake() {
        Instance = this;
    }

    [SerializeField] private Material glitchMaterial;
    [SerializeField] public float noiseAmount; // 0 is off 100 is full
    [SerializeField] public float glitchStrength; // 0 is off 100 is full
    [SerializeField] public float scanLinesStrength; // 1 is off 0 is full

    private Sequence glitchSequence;

    void Update()
    {
        glitchMaterial.SetFloat("_NoiseAmount", noiseAmount);
        glitchMaterial.SetFloat("_GlitchStrength", glitchStrength);
        glitchMaterial.SetFloat("_ScanLinesStrength", scanLinesStrength);   
    }


    public void SetGlitch(float noise, float glitch, float scanLines) {
        noiseAmount = noise;
        glitchStrength = glitch;
        scanLinesStrength = scanLines;
    }


    [Button]
    public void Glitch(float duration){

        if(glitchSequence != null){
            glitchSequence.Kill();
        }

        // Store original values
        float originalNoise = noiseAmount;
        float originalGlitch = glitchStrength;
        float originalScanLines = scanLinesStrength;
        float halfDuration = duration / 2;
        // Create a new DOTween sequence
        glitchSequence = DOTween.Sequence();

        // Append the tween to change all values to the glitch state
        glitchSequence.Append(DOTween.To(() => noiseAmount, x => noiseAmount = x, 100, halfDuration));
        glitchSequence.Join(DOTween.To(() => glitchStrength, x => glitchStrength = x, 100, halfDuration));
        glitchSequence.Join(DOTween.To(() => scanLinesStrength, x => scanLinesStrength = x, 0, halfDuration));

        // Append the tween to revert all values back to their original state
        glitchSequence.Append(DOTween.To(() => noiseAmount, x => noiseAmount = x, originalNoise, halfDuration));
        glitchSequence.Join(DOTween.To(() => glitchStrength, x => glitchStrength = x, originalGlitch, halfDuration));
        glitchSequence.Join(DOTween.To(() => scanLinesStrength, x => scanLinesStrength = x, originalScanLines, halfDuration));

        // Optionally, you can set the sequence to play automatically
        glitchSequence.Play();
    }
}
