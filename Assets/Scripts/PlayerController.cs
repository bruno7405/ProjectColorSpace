using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Transform CameraAnchor;
    public float MouseSensitivity = 0.25f;
    public float Speed = 10f;
    public float Gravity = -20f;
    public float JumpHeight = 1.5f;
    public float CrouchScale = 0.5f;

    private CharacterController m_CharacterController;
    float x, y;
    float camX, camY;
    float verticalVelocity;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInput();
        Look();

        HandleMovement();
    }

    private void MyInput()
    {
        x = 0f;
        y = 0f;

        if (Keyboard.current.wKey.isPressed) y += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;

        camY += Mouse.current.delta.x.ReadValue() * MouseSensitivity;
        camX -= Mouse.current.delta.y.ReadValue() * MouseSensitivity;
    }

    private void Look()
    {
        camX = Mathf.Clamp(camX, -90f, 90f);

        CameraAnchor.localRotation = Quaternion.Euler(camX, 0, 0);
        transform.rotation = Quaternion.Euler(0, camY, 0);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * x + transform.forward * y;
        move.Normalize();

        bool grounded = m_CharacterController.isGrounded;

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (grounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        verticalVelocity += Gravity * Time.deltaTime;

        Vector3 velocity = move * Speed;
        velocity.y = verticalVelocity;

        m_CharacterController.Move(velocity * Time.deltaTime);
    }
}
