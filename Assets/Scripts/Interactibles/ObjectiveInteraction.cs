using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveInteraction : Interactible
{
    [SerializeField] private bool defaultInteractible = true;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private GameObject objective;

    private bool interactible= false;

    private void Start() {
        interactible = defaultInteractible;
    }
    
    public override void Interact()
    {
        GameManager.Instance.ObjectiveCompleted(objective);
        interactible = false;
        objective.SetActive(false);

        if(onInteract != null)
            onInteract.Invoke();
    }

    public override bool IsInteractible()
    {
        return interactible;
    }

    public override void ToggleInteractible(bool value)
    {
        interactible = value;
    }
}   
