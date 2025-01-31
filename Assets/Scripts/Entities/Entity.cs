using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [FoldoutGroup("Entity State"), SerializeField] public bool isPaused = false;

    public void Pause() {
        isPaused = true;
    }

    public void Resume() {
        isPaused = false;
    }

    public virtual void Update() {
        if (isPaused) {
            return;
        }
    }
}
