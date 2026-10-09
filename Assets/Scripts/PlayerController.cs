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

    private Door currentPlatform;
    private MovingPlatform currentMovingPlatform;

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
        AudioBus.Instance.PlayRandomSFX(footstepClips);
        yield return new WaitForSeconds(footstepInterval);
        steppingCoroutine = null;
    }

    

    private void HandleMovement()
    {
        Vector3 move = transform.right * x + transform.forward * y;
        move.Normalize();
        
        bool grounded = m_CharacterController.isGrounded;

        if (grounded) _coyoteTimer = CoyoteTime;
        else _coyoteTimer -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame) _jumpBufferTimer = JumpBufferTime;
        else _jumpBufferTimer -= Time.deltaTime;

        if (grounded && currentGround != null && currentGround.GetComponent<BouncePad>() != null)
        {
            grounded = false;
            _coyoteTimer = 0f;
            BouncePad pad = currentGround.GetComponent<BouncePad>();
            verticalVelocity = pad.bouncePower;
            AudioBus.Instance.PlaySFX(bounceAudio);
        }

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            AudioBus.Instance.PlaySFX(jumpAudio);
            _jumpBufferTimer = 0f; 
            _coyoteTimer = 0f;
        }

        if (!grounded)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }

        velocity = move * Speed;
        velocity.y = verticalVelocity;

        Vector3 platformDelta = Vector3.zero;
        if (grounded && currentPlatform != null)
            platformDelta = currentPlatform.DeltaMovement;
        else if (grounded && currentMovingPlatform != null)
            platformDelta = currentMovingPlatform.DeltaMovement;
        else if (!grounded)
            currentPlatform = null;
        currentMovingPlatform = null;

        if (grounded && steppingCoroutine == null && (Mathf.Abs(velocity.x) > 0f || Mathf.Abs(velocity.z) > 0f))
        {
            steppingCoroutine = StartCoroutine(WaitForStep());
        }

        m_CharacterController.Move(velocity * Time.deltaTime + platformDelta);
    }
 

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.7f)
        {
            currentGround = hit.collider.gameObject;
            if (hit.collider.GetComponentInParent<Door>() != null)
            {
                currentPlatform = hit.collider.GetComponentInParent<Door>();
                currentMovingPlatform = null;
            }
            else if (hit.collider.GetComponent<Door>() != null)
            {
                currentPlatform = hit.collider.GetComponent<Door>();
                currentMovingPlatform = null;
            }
            else if (hit.collider.GetComponentInParent<MovingPlatform>() != null)
            {
                currentMovingPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
                currentPlatform = null;
            }
            else if(hit.collider.GetComponent<MovingPlatform>() != null)
            {
                currentMovingPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
                currentPlatform = null;
            }
            
        }
    }
}
