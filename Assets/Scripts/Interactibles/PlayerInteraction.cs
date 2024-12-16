using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;

    private void Awake() {
        Instance = this;
    }
    public List<Interactible> nearbyInteractibles = new List<Interactible>();

    [SerializeField] private GameObject interactionDisplay;
    void Start()
    {
        interactionDisplay.SetActive(false);
    }

    void Update()
    {
        bool setInteractible = false;

        var closestInteractible = nearbyInteractibles.OrderBy(interactible => Vector3.Distance(transform.position, interactible.transform.position)).FirstOrDefault();
        if (closestInteractible != null)
        {
            if (closestInteractible.IsInteractible())
            {
                setInteractible = true;
            }
        }

        interactionDisplay.SetActive(setInteractible);

        if (setInteractible && Input.GetKeyDown(KeyCode.E))
        {
            closestInteractible.Interact();
        }
    }
}
