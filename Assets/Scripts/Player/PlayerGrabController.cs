using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrabController : MonoBehaviour
{
    public static PlayerGrabController Instance { get; private set; }

    [Header("Grab Variables")]
    [SerializeField] Transform _grabTarget;
    [SerializeField] float _grabDistance = 3f;
    [SerializeField] float _followSpeed = 20f;
    [SerializeField] LayerMask _grabbableLayer;

    [Header("Clipping Prevention")]
    [SerializeField] LayerMask _obstacleMask; // everything NOT player or grabbable layers
    [SerializeField] float _dropPadding = 0.3f; // roughly half the object's size


    public static Action OnObjectHoverEntered;
    public static Action OnObjectHoverExited;
    public static Action OnObjectGrabbed;
    public static Action OnObjectDropped;

    private GrabbableObject _heldGrabbable;
    private GrabbableObject _hoveredGrabbable;
    private Transform _cam;

    public bool HasGrabbable => _hoveredGrabbable != null;
    public bool IsHolding => _heldGrabbable != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _cam = Camera.main.transform;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        

        if (Keyboard.current.fKey.wasPressedThisFrame) ToggleGrab();
    }

    private void LateUpdate()
    {
        if (_heldGrabbable == null) CheckForHover();

        if (_heldGrabbable != null) MoveHeldObject();
    }

    /// <summary>
    /// Move held object toward target position (lerp)
    /// </summary>
    private void MoveHeldObject()
    {

        Transform heldTransform = _heldGrabbable.transform;

        if (Vector3.Distance(heldTransform.position, _grabTarget.position) < 0.05f) return;

        float t = 1f - Mathf.Exp(-_followSpeed * Time.deltaTime);


        heldTransform.position = Vector3.Lerp(heldTransform.position, _grabTarget.position, t);

        // held.rotation = Quaternion.Slerp(held.rotation, grabPosition.rotation, t);
    }

    /// <summary>
    /// Checks if the player is hovering / looking at a grabbable object
    /// </summary>
    private void CheckForHover()
    {
        GrabbableObject found = null;

        if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, _grabDistance,
                _grabbableLayer, QueryTriggerInteraction.Ignore))
        {
            found = hit.collider.GetComponentInParent<GrabbableObject>();
        }

        if (found == _hoveredGrabbable) return;

        if (_hoveredGrabbable != null)
        {
            _hoveredGrabbable.HoverExit();
            Debug.Log("hover exit");
            OnObjectHoverExited?.Invoke();
        }

        _hoveredGrabbable = found;

        if (_hoveredGrabbable != null)
        {
            OnObjectHoverEntered?.Invoke();
            Debug.Log("hover enter");
            _hoveredGrabbable.HoverEnter();
        }
    }

    /// <summary>
    /// Grabs or drops the currently grabbed object
    /// </summary>
    public void ToggleGrab()
    {
        // Drop item
        if (_heldGrabbable != null)
        {
            StopClipping();
            OnObjectDropped?.Invoke();
            _heldGrabbable.Dropped();
            _heldGrabbable = null;
            return;
        }

        if (_hoveredGrabbable == null) return;

        // Pick up item
        _heldGrabbable = _hoveredGrabbable;
        _hoveredGrabbable.HoverExit();
        OnObjectHoverExited?.Invoke();
        _hoveredGrabbable = null;
        _heldGrabbable.Grabbed();
        OnObjectGrabbed?.Invoke();

    }

    /// <summary>
    /// Pulls the held object back in front of any wall between the camera and it.
    /// Only call right before dropping.
    /// </summary>
    private void StopClipping()
    {
        Transform held = _heldGrabbable.transform;
        Vector3 origin = _cam.position;
        Vector3 toHeld = held.position - origin;
        float dist = toHeld.magnitude;
        if (dist < 0.001f) return;

        Vector3 dir = toHeld / dist;

        // dist + padding catches walls just behind the object's center, where its edge would be clipping
        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist + _dropPadding,
                _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            held.position = hit.point - dir * _dropPadding;
        }
    }
}