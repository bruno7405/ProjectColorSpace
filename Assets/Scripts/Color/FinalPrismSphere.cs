using System.Collections;
using UnityEngine;

public class FinalPrismSphere : MonoBehaviour
{
    public float Duration = 1.5f;
    public float Delay = 1f;
    public float StrenthMult = 1;

    public float maxScale = 10f;
    public AnimationCurve scaleCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 3f, 3f),
        new Keyframe(1f, 1f, 0f, 0f));

    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private Renderer _renderer;
    private MaterialPropertyBlock _block;
    private int _fadeID;
    private int _progressID;
    private Coroutine routine;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
        //_fadeID = Shader.PropertyToID(fadeProperty);
        //_progressID = Shader.PropertyToID(progressProperty);
        SetVisible(false);
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        do
        {
            SetVisible(true);

            float elapsed = 0f;
            while (elapsed < Duration)
            {
                float t = elapsed / Duration;
                Apply(t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            Apply(1f);
            SetVisible(false);

            yield return new WaitForSeconds(Delay);
        }
        while (true);

        routine = null;
    }

    private void Apply(float t)
    {
        float s = scaleCurve.Evaluate(t) * maxScale;
        transform.localScale = Vector3.one * s;

        _renderer.GetPropertyBlock(_block);
        //_block.SetFloat(progressId, t);
        _block.SetFloat("_Strength", StrenthMult * FadeCurve.Evaluate(t));
        _renderer.SetPropertyBlock(_block);
    }

    private void SetVisible(bool visible)
    {
        _renderer.enabled = visible;
    }
}