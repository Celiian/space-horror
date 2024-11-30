using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectLightParameters : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    private ParticleSystem lightParticleSystem;

    private void Start()
    {
        lightParticleSystem = GetComponent<ParticleSystem>();
        var shape = lightParticleSystem.shape;
        shape.radius = radius;
    }
    


    [Button]
    public void StartDebugDrawPropagationDistanceCoroutine()
    {
        StartCoroutine(DebugDrawPropagationDistanceCoroutine());
    }


    private void  DebugDrawPropagationDistance()
    {
        int segments = 12;  
        float angle = 0f;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            Vector3 start = transform.position + new Vector3(x, y, 0);
            angle += angleStep;
            x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            Vector3 end = transform.position + new Vector3(x, y, 0);
            Debug.DrawLine(start, end, Color.red);
        }
    }


    private IEnumerator DebugDrawPropagationDistanceCoroutine()
    {
        int i = 0;
        while (i < 10)
        {
            DebugDrawPropagationDistance();
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }

  
}
