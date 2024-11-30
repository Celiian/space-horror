using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu(){
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    private void ResumeGame(){
        TogglePauseMenu();
    }
}
