using System.Collections.Generic;
using UnityEngine;

public interface IMovingSurface
{
    Vector3 DeltaMovement { get; }
}

[DefaultExecutionOrder(-100)]
public class MovingPlatform : MonoBehaviour, IMovingSurface
{
    [SerializeField] Transform doorTransform;
    [SerializeField] float doorSpeed = 10f;
    [SerializeField] float pauseDuration = 1f;

    public List<Transform> moveTargetTransforms = new List<Transform>();

    Vector3 originalPosition;
    List<Vector3> offsets = new List<Vector3>();
    List<float> progress = new List<float>();
    List<bool> reachedEnd = new List<bool>();
    List<bool> axisActive = new List<bool>();
    int activeIndex = 0;
    float pauseTimer = 0f;

    public Vector3 DeltaMovement { get; private set; }
    Vector3 lastPosition;

    void Awake()
    {
        originalPosition = doorTransform.position;

        for (int i = 0; i < moveTargetTransforms.Count; i++)
        {
            offsets.Add(moveTargetTransforms[i].position - originalPosition);
            progress.Add(0f);
            reachedEnd.Add(false);
            axisActive.Add(false);
        }

        lastPosition = doorTransform.position;
    }

    public void SetTarget(int index)
    {
        if (index < 0 || index >= offsets.Count) return;

        if (index != activeIndex)
        {
            pauseTimer = 0f;
            activeIndex = index;
        }
    }

    public void EnableAxis(int index)
    {
        axisActive[index] = true;
    }

    public void DisableAxis(int index)
    {
        axisActive[index] = false;
    }

    void Update()
    {
        if (offsets.Count > 0)
        {
            if (axisActive[activeIndex])
            {
                if (pauseTimer > 0f)
                {
                    pauseTimer -= Time.deltaTime;
                }
                else
                {
                    float length = offsets[activeIndex].magnitude;
                    if (length > 0.0001f)
                    {
                        float target = reachedEnd[activeIndex] ? 0f : 1f;
                        float step = doorSpeed * Time.deltaTime / length;
                        progress[activeIndex] = Mathf.MoveTowards(progress[activeIndex], target, step);

                        if (Mathf.Approximately(progress[activeIndex], target))
                        {
                            reachedEnd[activeIndex] = !reachedEnd[activeIndex];
                            pauseTimer = pauseDuration;
                        }
                    }
                }
            }

            Vector3 total = Vector3.zero;
            for (int i = 0; i < offsets.Count; i++)
                total += offsets[i] * progress[i];

            doorTransform.position = originalPosition + total;
        }

        DeltaMovement = doorTransform.position - lastPosition;
        lastPosition = doorTransform.position;

        if (DeltaMovement != Vector3.zero)
            Physics.SyncTransforms();
    }
}