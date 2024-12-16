using DG.Tweening;
using UnityEngine; // Make sure to include the DOTween namespace

public class UiPulse : MonoBehaviour
{
    public RectTransform uiElement; // Reference to the UI element you want to scale

    void Start()
    {
        // Start the pulsing animation
        Pulse();
    }

    void Pulse()
    {
        // Animate the scale to 1.1 over 0.5 seconds, then back to 0.9 over 0.5 seconds, and loop
        uiElement.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
