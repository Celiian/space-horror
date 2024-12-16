using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool isPaused = false;
    

    public virtual void Update() {
        Debug.Log(name);
        Debug.Log(isPaused);
        if (isPaused) {
            return;
        }
    }
}
