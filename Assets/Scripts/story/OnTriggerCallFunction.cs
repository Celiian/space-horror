using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class OnTriggerCallFunction : MonoBehaviour
{
    [SerializeField] private List<triggerFunction> triggerFunctions;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(var triggerFunction in triggerFunctions)
            {
                if(!triggerFunction.isInvoked)
                {
                    triggerFunction.functionToCall.Invoke();
                    triggerFunction.isInvoked = true;
                }
            }
        }
    }
}

[System.Serializable]
public class triggerFunction
{
    public UnityEvent functionToCall;
    public bool invokeOnce = false;
    public bool isInvoked = false;
}