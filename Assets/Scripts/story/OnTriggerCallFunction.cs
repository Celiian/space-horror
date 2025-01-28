using UnityEngine;
using UnityEngine.Events;

public class OnTriggerCallFunction : MonoBehaviour
{
    [SerializeField] private UnityEvent functionToCall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            functionToCall.Invoke();
        }
    }
}