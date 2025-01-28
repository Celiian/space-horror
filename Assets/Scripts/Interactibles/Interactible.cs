using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public bool isInteractible = true;
    public abstract void Interact();

    public abstract bool IsInteractible();

    public abstract void ToggleInteractible(bool value);


    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction.Instance.nearbyInteractibles.Add(this);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction.Instance.nearbyInteractibles.Remove(this);
        }
    }
}
