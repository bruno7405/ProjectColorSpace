using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Transform CameraAnchor;
    public float MouseSensitivity = 0.25f;
    public float Speed = 10f;
    public float Gravity = -20f;
    public float JumpHeight = 0f;
    public float CrouchScale = 0.5f;

    private Vector3 velocity;

    private GameObject currentGround;

    private CharacterController m_CharacterController;
    float x, y;
    float camX, camY;
    float verticalVelocity;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (GameManager.CurrentState != GameState.Playing) return;
        
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

        if (grounded && currentGround != null && currentGround.GetComponent<BouncePad>() != null)
        {
            grounded = false;
            BouncePad pad = currentGround.GetComponent<BouncePad>();
            verticalVelocity = pad.bouncePower;
        }

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (grounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        if (!grounded)
        {
            verticalVelocity += Gravity * Time.deltaTime;
            transform.SetParent(null);
        }

        if (grounded && currentGround != null && currentGround.layer == LayerMask.NameToLayer("Moving Platform"))
        {
            Vector3 groundVelocity = currentGround.GetComponent<Door>().CalculatedVelocity;
            velocity = (move * Speed) + groundVelocity;
            velocity.y = verticalVelocity + groundVelocity.y;
        }

        velocity = move * Speed;
        velocity.y = verticalVelocity;

        m_CharacterController.Move(velocity * Time.deltaTime);
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.7f)
        {
            currentGround = hit.collider.gameObject;
        }
    }
}
