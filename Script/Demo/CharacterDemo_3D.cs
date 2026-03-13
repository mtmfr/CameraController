using CameraController;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterDemo_3D : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] Camera3D cam;

    private Rigidbody rb;

    Vector3 movementDir;

    private float speed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    private void Update()
    {
        Quaternion cameraYaw = cam.yaw;

        Vector3 relativeDir = cameraYaw * movementDir;

        rb.linearVelocity = relativeDir * speed;
    }

    private void GetMovementDir(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        movementDir = new Vector3(value.x, 0, value.y);
    }

}
