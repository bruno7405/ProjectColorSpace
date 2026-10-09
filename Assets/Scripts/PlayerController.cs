using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

[DefaultExecutionOrder(100)]
public class PlayerController : MonoBehaviour
{

    public Transform CameraAnchor;
    public float MaxMouseSensitivity = 0.25f;
    public float Speed = 10f;
    public float Gravity = -20f;
    public float JumpHeight = 0f;
    public float CrouchScale = 0.5f;
    public float CoyoteTime = 0.15f;
    public float JumpBufferTime = 0.1f;
    private float _jumpBufferTimer;
    
    private float _coyoteTimer;

    private Vector3 velocity;

    private GameObject currentGround;

    private CharacterController m_CharacterController;
    float x, y;
    float camX, camY;
    float verticalVelocity;

    [Header("Audio")]
    public float footstepInterval = 0.45f; // Interval in seconds.
    public AudioClip[] footstepClips;
    public AudioClip jumpAudio;
    public AudioClip landAudio;
    public AudioClip bounceAudio;

    private IMovingSurface surface;
    private float surfaceTimer;
    private const float SurfaceGrace = 0.1f;

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

        camY += Mouse.current.delta.x.ReadValue() * MaxMouseSensitivity * GameSettings.MouseSensitivity;
        camX -= Mouse.current.delta.y.ReadValue() * MaxMouseSensitivity * GameSettings.MouseSensitivity;
    }

    private void Look()
    {
        camX = Mathf.Clamp(camX, -90f, 90f);

        CameraAnchor.localRotation = Quaternion.Euler(camX, 0, 0);
        transform.rotation = Quaternion.Euler(0, camY, 0);
    }

    private Coroutine steppingCoroutine = null;

    IEnumerator WaitForStep() {
        AudioBus.Instance.PlayRandomSFX(footstepClips, 0.75f);
        yield return new WaitForSeconds(footstepInterval);
        steppingCoroutine = null;
    }

    private bool prevGrounded = false;
    private float prevVelocityY = 0.0f;

    private void HandleMovement()
    {
        float dt = Time.deltaTime;

        bool grounded = m_CharacterController.isGrounded;

        surfaceTimer -= dt;
        if (surfaceTimer <= 0f) surface = null;
        if (surface != null)
            m_CharacterController.Move(surface.DeltaMovement);

        Vector3 move = (transform.right * x + transform.forward * y).normalized;

        if (grounded && !prevGrounded && Mathf.Abs(prevVelocityY) > 5.5f) AudioBus.Instance.PlaySFX(landAudio);
        prevGrounded = grounded;

        if (grounded) _coyoteTimer = CoyoteTime;
        else _coyoteTimer -= dt;

        if (Keyboard.current.spaceKey.wasPressedThisFrame) _jumpBufferTimer = JumpBufferTime;
        else _jumpBufferTimer -= dt;

        if (grounded && currentGround != null && currentGround.TryGetComponent(out BouncePad pad))
        {
            grounded = false;
            _coyoteTimer = 0f;
            verticalVelocity = pad.bouncePower;
            surface = null;
            AudioBus.Instance.PlaySFX(bounceAudio);
        }

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            AudioBus.Instance.PlaySFX(jumpAudio);
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
            surface = null; 
        }

        if (!grounded)
            verticalVelocity += Gravity * dt;

        velocity = move * Speed;
        velocity.y = verticalVelocity;
        prevVelocityY = velocity.y;

        if (grounded && steppingCoroutine == null && move.sqrMagnitude > 0f)
            steppingCoroutine = StartCoroutine(WaitForStep());

        m_CharacterController.Move(velocity * dt);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.7f)
        {
            currentGround = hit.collider.gameObject;
            surface = hit.collider.GetComponentInParent<IMovingSurface>();
            surfaceTimer = SurfaceGrace;
        }
    }
}
