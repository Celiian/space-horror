using UnityEngine;

public static class CalculateSoundVolume
{
    
    public class SoundParameters
    {
        public float MaxVolume = 0.5f;
        public float MinVolume = 0f;
        public float MinCutoffFrequency = 1000f;
        public float MaxCutoffFrequency = 5000f;
        public float MaxSpatialBlend = 1f;
        public float MinSpatialBlend = 0.5f;
        public float MinDistance = 4f;
        public float MaxDistance;
        public float MaxSpeed;

        public SoundParameters(float maxDistance, float maxSpeed)
        {
            MaxDistance = maxDistance;
            MaxSpeed = maxSpeed;
        }
    }

    public static (float volume, float cutoffFrequency, float spatialBlend) CalculateSoundProperties(
        float distance, int numberOfWalls, bool isPlayer, SoundParameters parameters, float maxVolume, Rigidbody2D rb = null)
    {
        float distanceFactor = Mathf.Clamp01((parameters.MaxDistance - distance) / (parameters.MaxDistance - parameters.MinDistance));
        float wallFactor = Mathf.Clamp01(1 - (numberOfWalls / 3f));
        float combinedFactor = distanceFactor * wallFactor;

        float speedFactor = 1f;
        if(rb != null){
            // Speed factor based on object's velocity
            speedFactor = Mathf.Clamp01(rb.velocity.magnitude / parameters.MaxSpeed);
        }
    
        // Volume combines speed and other factors
        float volume = maxVolume * speedFactor;

        if (isPlayer)
        {
            volume = Mathf.Clamp(volume, parameters.MinVolume, parameters.MaxVolume);
            return (volume, parameters.MaxCutoffFrequency, parameters.MaxSpatialBlend);
        }

        volume *= combinedFactor;
        volume = Mathf.Clamp(volume, parameters.MinVolume, parameters.MaxVolume);
        // Cutoff frequency and spatial blend adjusted for combined and speed factors
        float cutoffFrequency = Mathf.Lerp(parameters.MaxCutoffFrequency, parameters.MinCutoffFrequency, 1 - combinedFactor);
        float spatialBlend = Mathf.Lerp(parameters.MaxSpatialBlend, parameters.MinSpatialBlend, 1 - combinedFactor);

        cutoffFrequency = Mathf.Clamp(cutoffFrequency, parameters.MinCutoffFrequency, parameters.MaxCutoffFrequency);
        spatialBlend = Mathf.Clamp(spatialBlend, parameters.MinSpatialBlend, parameters.MaxSpatialBlend);

        return (volume, cutoffFrequency, spatialBlend);
    }


    public static float CalculateLightIntensity(float distance, float maxDistance, float minDistance, float minLightIntensity, float maxLightIntensity, float wallCount)
    {
        float distanceFactor = Mathf.Clamp01((maxDistance - distance) / (maxDistance - minDistance));
        float wallFactor = Mathf.Clamp01(1 - (wallCount / 4f));
        return Mathf.Lerp(minLightIntensity, maxLightIntensity, distanceFactor * wallFactor);
    }
}
