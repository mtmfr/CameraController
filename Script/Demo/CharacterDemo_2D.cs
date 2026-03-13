using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class CharacterDemo_2D : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D rb;

    Vector2 movementDir;

    private float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        moveAction.action.performed += GetMovementDir;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        moveAction.action.performed -= GetMovementDir;
    }

    void Update()
    {
        Vector2 velocity = new(movementDir.x * speed, rb.linearVelocity.y);
        rb.linearVelocity = velocity;
    }

    private void GetMovementDir(InputAction.CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
    }
}
