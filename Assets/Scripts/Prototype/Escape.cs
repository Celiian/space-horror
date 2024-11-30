using UnityEngine.SceneManagement;
using UnityEngine;

public class Escape : MonoBehaviour
{
    void Awake() => Instance = this;
    public static Escape Instance { get; private set; }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && GameManager.Instance.canEscape){
            Debug.Log("Player escaped");
            SceneManager.LoadScene("MainMenu");
        }
    }
}