using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] public Transform target;

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
}
