using System.Collections.Generic;
using Febucci.UI;
using Febucci.UI.Core;
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

    private bool didSkip = false;

    private bool isTypewriterFinished = false;

    private TypewriterCore typewriter;
    private TAnimCore textAnimator;

    private void Start() {
        interactible = defaultInteractible;
        typewriter = terminalPageText.GetComponent<TypewriterByCharacter>();
        textAnimator = terminalPageText.GetComponent<TAnimCore>();
    }
    
    public override void Interact()
    {
        isInteractible = false;
        isTypewriterFinished = false;
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
        typewriter.ShowText(pages[currentPageIndex]);

        if(onInteract != null)
            onInteract.Invoke();
            onInteract = null;
        UpdateButtons();
    }

    public override bool IsInteractible()
    {
        return interactible && !isOpen;
    }

    public override void ToggleInteractible(bool value)
    {
        interactible = value;
    }

    private void Update() {
        if (!isOpen) return;
        if (typewriter.TextAnimator.allLettersShown) {
            isTypewriterFinished = true;
        }

        if (PlayerInteraction.Instance.didInteract && !isTypewriterFinished) {
            didSkip = true;
            typewriter.SkipTypewriter();
            PlayerInteraction.Instance.didInteract = false;
        }
        else if (PlayerInteraction.Instance.didInteract && (didSkip || isTypewriterFinished)) {
            isOpen = false;
            didSkip = false;
            isTypewriterFinished = false;
            foreach (var entity in entitiesToUnpause) {
                entity.isPaused = false;
            }
            entitiesToUnpause.Clear();
            onInteractFinished?.Invoke();
            onInteractFinished = null;
            PlayerInteraction.Instance.didInteract = false;
            textAnimator.SetText("");
            terminal.SetActive(false);
        }
        
        if (PlayerInteraction.Instance.didPressNext) {
            currentPageIndex = (currentPageIndex + 1) % pages.Length;
            terminalPageText.text = pages[currentPageIndex];
            UpdateButtons();
            PlayerInteraction.Instance.didPressNext = false;
        }
        if (PlayerInteraction.Instance.didPressPrevious) {
            currentPageIndex = (currentPageIndex - 1 + pages.Length) % pages.Length;
            terminalPageText.text = pages[currentPageIndex];
            UpdateButtons();
            PlayerInteraction.Instance.didPressPrevious = false;
        }
    }


    private void UpdateButtons() {
        nextPageButton.SetActive(currentPageIndex < pages.Length - 1);
        previousPageButton.SetActive(currentPageIndex > 0);
    }
}

internal class TextAnimator
{
}