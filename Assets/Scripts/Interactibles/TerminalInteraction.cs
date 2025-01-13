using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TerminalInteraction : Interactible
{
    [SerializeField] private GameObject terminal;
    [SerializeField] private bool defaultInteractible = true;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onInteractFinished;
    [SerializeField] private TextMeshProUGUI terminalPageText;
    [SerializeField] private TextMeshProUGUI terminalNameText;
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private string terminalName;
    [SerializeField] private string[] pages;

    private int currentPageIndex = 0;
    private bool isOpen = false;

    private bool interactible= false;

    private List<Entity> entitiesToUnpause = new List<Entity>();

    private void Start() {
        interactible = defaultInteractible;
    }
    
    public override void Interact()
    {
        isOpen = true;
        terminal.SetActive(true);
        Entity[] entities =  FindObjectsOfType<Entity>();
        foreach (var entity in entities) {
            if(!entity.isPaused){
                entitiesToUnpause.Add(entity);
                entity.isPaused = true;
            }
        }

        terminalNameText.text = terminalName;
        terminalPageText.text = pages[currentPageIndex];
        if(onInteract != null)
            onInteract.Invoke();
        UpdateButtons();
    }

    public override bool IsInteractible()
    {
        return interactible;
    }

    public override void ToggleInteractible(bool value)
    {
        interactible = value;
    }

    private void Update() {
        if (!isOpen) return;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isOpen = false;
            terminal.SetActive(false);
            foreach (var entity in entitiesToUnpause) {
                entity.isPaused = false;
            }
            entitiesToUnpause.Clear();
            onInteractFinished?.Invoke();
        }
         if (Input.GetKeyDown(KeyCode.Space)) {
            terminalPageText.GetComponent<TypewriterByCharacter>().SkipTypewriter();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentPageIndex = (currentPageIndex + 1) % pages.Length;
            terminalPageText.text = pages[currentPageIndex];
            UpdateButtons();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentPageIndex = (currentPageIndex - 1 + pages.Length) % pages.Length;
            terminalPageText.text = pages[currentPageIndex];
            UpdateButtons();
        }
    }


    private void UpdateButtons() {
        nextPageButton.SetActive(currentPageIndex < pages.Length - 1);
        previousPageButton.SetActive(currentPageIndex > 0);
    }
}   