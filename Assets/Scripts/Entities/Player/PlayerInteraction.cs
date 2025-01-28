using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;

    private void Awake() {
        Instance = this;
    }
    public List<Interactible> nearbyInteractibles = new List<Interactible>();

    [SerializeField] private GameObject interactionDisplay;
    public bool didInteract = false;
    public bool didPressNext = false;
    public bool didPressPrevious = false;

    private Interactible closestActivatedInteractible;
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

        if (setInteractible)
        {
            closestActivatedInteractible = closestInteractible;
        }
        else
        {
            closestActivatedInteractible = null;
        }
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if(context.performed){
            didInteract = true;
            if (closestActivatedInteractible != null && closestActivatedInteractible.IsInteractible()) {
                closestActivatedInteractible.Interact();
                didInteract = false;
                closestActivatedInteractible = null;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        if(context.performed) {
            if(context.ReadValue<Vector2>().x > 0) {
                didPressNext = true;
            }
            else if(context.ReadValue<Vector2>().x < 0) {
                didPressPrevious = true;
            }
            StartCoroutine(ResetInputs());
        }
    }

    private IEnumerator ResetInputs() {
        yield return new WaitForSeconds(0.1f);
        didPressNext = false;
        didPressPrevious = false;
    }
}