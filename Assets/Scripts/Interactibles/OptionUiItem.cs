using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionUiItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private Button optionButton;


    public void Init(Option option)
    {
        optionText.text = option.name;
        optionButton.onClick.AddListener(() => option.action.Invoke());
    }
}
