using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;

    private void Awake() {
        Instance = this;
    }


    private void GetInput() {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputVector.magnitude > 1f) {
            inputVector.Normalize();
        }
        rb.velocity = inputVector * speed;
    }


    private void FixedUpdate() {
        GetInput();
    }
}
