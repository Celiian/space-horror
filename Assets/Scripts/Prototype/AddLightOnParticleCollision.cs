using UnityEngine;

public class AddLightOnParticleCollision : MonoBehaviour
{
    [SerializeField] private GameObject _lightPrefab;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collision");
        Debug.Log(other.transform.name);
        Vector3 collisionPoint = other.transform.position;
        Instantiate(_lightPrefab, collisionPoint, Quaternion.identity);
    }
}
