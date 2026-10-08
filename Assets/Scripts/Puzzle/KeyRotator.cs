using UnityEngine;

public class KeyRotator : MonoBehaviour
{
    public float HoverSpeed = 1f;
    public float HoverAmplitude = 1f;

    public Vector3 RotationSpeeds = new Vector3(10f, 20f, 60f);

    private Quaternion _initialRot;
    private Vector3 _euler;
    private Vector3 _initialPos;

    void Start()
    {
        UpdateInitalPositionAndRotation();
    }

    public void UpdateInitalPositionAndRotation()
    {
        _initialPos = transform.position;
        _initialRot = transform.rotation;
        _euler = Vector3.zero;
    }

    void Update()
    {
        Vector3 pos = _initialPos;
        pos.y += Mathf.Sin(Time.time * HoverSpeed) * HoverAmplitude;
        transform.position = pos;

        _euler += RotationSpeeds * Time.deltaTime;
        transform.rotation = _initialRot * Quaternion.Euler(_euler);
    }
}
