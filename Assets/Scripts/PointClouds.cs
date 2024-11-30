using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PointClouds : MonoBehaviour
{
    [SerializeField] public float ObjectX = 1f;            // Width of the area
    [SerializeField] public float ObjectY = 1f;            // Height of the area
    [SerializeField] int particlesPerSide = 3;  
    [SerializeField] bool borderOnly = true; // Control for border particles

    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private bool particleUpdated = false;

    private float particleSize = 0.05f; // Size of each particle

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        InitializeParticles();
    }

    void Update()
    {
        if (particleUpdated)
        {
            particleSystem.SetParticles(particles, particles.Length);
            particleUpdated = false;
        }
    }

    private void InitializeParticles()
    {
        // Calculate the number of particles along each axis based on particlesPerSide
        int particlesForX = (int)Mathf.Ceil(ObjectX * particlesPerSide); // Particles along width
        int particlesForY = (int)Mathf.Ceil(ObjectY * particlesPerSide); // Particles along height

        // Calculate total number of particles
        int totalParticles;

        // If borderOnly is true, calculate only border particles
        if (borderOnly)
        {
            totalParticles = (particlesForX * 2) + (particlesForY * 2) - 4; // Subtract corners counted twice
        }
        else
        {
            totalParticles = particlesForX * particlesForY;
        }

        particles = new ParticleSystem.Particle[totalParticles];

        // Calculate spacing based on the object dimensions and number of particles
        float spacingX = ObjectX / (particlesForX - 1); // Divide by number of particles in width
        float spacingY = ObjectY / (particlesForY - 1); // Divide by number of particles in height

        int index = 0;

        if (borderOnly)
        {
            // Create bottom border
            for (int i = 0; i < particlesForX; i++)
            {
                particles[index].position = new Vector3(-ObjectX / 2 + i * spacingX, -ObjectY / 2, 0); // Bottom border
                particles[index].startColor = Color.white; // Change color for visibility
                particles[index].startSize = particleSize; // Set size of particles
                index++;
            }

            // Create right border
            for (int j = 1; j < particlesForY - 1; j++) // Skip corners
            {
                particles[index].position = new Vector3(ObjectX / 2, -ObjectY / 2 + j * spacingY, 0); // Right border
                particles[index].startColor = Color.white; 
                particles[index].startSize = particleSize; 
                index++;
            }

            // Create top border
            for (int i = particlesForX - 1; i >= 0; i--)
            {
                particles[index].position = new Vector3(-ObjectX / 2 + i * spacingX, ObjectY / 2, 0); // Top border
                particles[index].startColor = Color.white; 
                particles[index].startSize = particleSize; 
                index++;
            }

            // Create left border
            for (int j = particlesForY - 2; j > 0; j--) // Skip corners
            {
                particles[index].position = new Vector3(-ObjectX / 2, -ObjectY / 2 + j * spacingY, 0); // Left border
                particles[index].startColor = Color.white; 
                particles[index].startSize = particleSize; 
                index++;
            }
        }
        else
        {
            // Position each particle in a grid pattern (original logic)
            for (int i = 0; i < particlesForX; i++)
            {
                for (int j = 0; j < particlesForY; j++)
                {
                    int gridIndex = i * particlesForY + j; // Correct index calculation for 2D grid
                    particles[gridIndex].position = new Vector3(-ObjectX / 2 + i * spacingX, -ObjectY / 2 + j * spacingY, 0); // Centered position
                    particles[gridIndex].startColor = Color.white; // Change color for visibility
                    particles[gridIndex].startSize = particleSize; // Set size of particles
                }
            }
        }

        particleUpdated = true;
    }
}
