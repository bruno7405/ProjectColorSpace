using UnityEngine;

public class PrismRotator : MonoBehaviour
{
    public float HoverSpeed = 1f;
    public float HoverAmplitude = 1f;

    public float TurnInterval = 3f;
    private Quaternion _target;
    private float _timer;
    private Vector3 _initialPos;

    void Start()
    {
        UpdateInitalPositionAndRotation();
    }
    
    public void UpdateInitalPositionAndRotation()
    {
        _initialPos = transform.position;
        _target = transform.rotation;
    }

    void Update()
    {
        Vector3 pos = _initialPos;
        pos.y += Mathf.Sin(Time.time * HoverSpeed) * HoverAmplitude;
        transform.position = pos;


        _timer -= Time.deltaTime;
        if (_timer <= 0f) { _target = Random.rotation; _timer = TurnInterval; }

        transform.rotation = Quaternion.Slerp(transform.rotation, _target, Time.deltaTime * 1.5f);
    }
}
