using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public struct Option
{
    public string name;
    public string variableName;
    public UnityEvent action;
}


public abstract class IInteractible : MonoBehaviour
{
    public abstract void Interact();
    public abstract bool IsInteractible();

    // [SerializeField] private bool isInteractible = false;
    [SerializeField] public List<Option> options;
    [SerializeField] public Dictionary<string, bool> toggleableOptions;
    [SerializeField] private GameObject optionUiItemPrefab;
    [SerializeField] private GameObject interactibleDisplay;
    [SerializeField] private Dictionary<string, GameObject> optionsUiItems = new Dictionary<string, GameObject>();
    
    public void Init()
    {
        toggleableOptions = new Dictionary<string, bool>();
        foreach (Option option in options)
        {
            OptionUiItem uiItem = Instantiate(optionUiItemPrefab, interactibleDisplay.transform).GetComponent<OptionUiItem>();
            uiItem.Init(option);
            optionsUiItems.Add(option.variableName, uiItem.gameObject);
        }
        interactibleDisplay.SetActive(false);
    }

    public void CheckOptions()
    {
        foreach (Option option in options)
        {
            if (toggleableOptions.ContainsKey(option.variableName))
            {
                optionsUiItems[option.variableName].SetActive(toggleableOptions[option.variableName]);
            }
        }
    }

    public void ToggleInteractible()
    {
    }


    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactibleDisplay.SetActive(true);
            CheckOptions();
        }
    }


    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactibleDisplay.SetActive(false);
        }
    }

    private void Update()
    {
        if (interactibleDisplay.activeSelf)
        {
            var position = PlayerMovement.Instance.transform.position;
            interactibleDisplay.transform.position = new Vector3(position.x + 2.3f, position.y, position.z);
        }
    }

}
