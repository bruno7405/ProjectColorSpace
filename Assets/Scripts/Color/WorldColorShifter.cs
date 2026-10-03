using UnityEngine;

public class WorldColorShifter : MonoBehaviour
{
    [SerializeField] private string materialPath = "Materials/WorldMaterial";

    [Header("Which properties the spectrum value drives")]
    [SerializeField] private bool driveBaseColor = true;
    [SerializeField] private bool driveShadedColor = true;
    [SerializeField] private bool driveRimColor = false;
    [SerializeField] private bool driveGradientColor = false;

    [SerializeField, Range(0f, 1f)] private float shadedBrightness = 0.85f;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorDimId = Shader.PropertyToID("_ColorDim");
    private static readonly int ColorDimStepsId = Shader.PropertyToID("_ColorDimSteps");
    private static readonly int ColorDimCurveId = Shader.PropertyToID("_ColorDimCurve");
    private static readonly int RimColorId = Shader.PropertyToID("_FlatRimColor");
    private static readonly int GradientColorId = Shader.PropertyToID("_ColorGradient");

    private static readonly int[] AllColorIds =
    {
        BaseColorId, ColorDimId, ColorDimStepsId, ColorDimCurveId, RimColorId, GradientColorId
    };

    private Material _material;
    private Color[] _originalColors;

    private void Awake()
    {
        LoadMaterial();
    }

    private void LoadMaterial()
    {
        _material = Resources.Load<Material>(materialPath);
        if (_material == null)
        {
            Debug.LogError($"Material not found at Resources/{materialPath}", this);
            enabled = false;
            return;
        }

        _originalColors = new Color[AllColorIds.Length];
        for (int i = 0; i < AllColorIds.Length; i++)
            if (_material.HasProperty(AllColorIds[i]))
                _originalColors[i] = _material.GetColor(AllColorIds[i]);
    }

    private void OnEnable()
    {
        if (_material != null)
            SpectrumManager.OnColorUpdate += HandleColorUpdate;
    }

    private void OnDisable()
    {
        SpectrumManager.OnColorUpdate -= HandleColorUpdate;
    }

    private void OnDestroy()
    {
        RestoreOriginalColors();
    }

    private void HandleColorUpdate(float value)
    {
        Color color = ColorUtilities.FloatToColor(value);

        if (driveBaseColor)
            _material.SetColor(BaseColorId, color);

        if (driveShadedColor)
        {
            Color shaded = color * shadedBrightness;
            shaded.a = 1f;
            _material.SetColor(ColorDimId, shaded);
            _material.SetColor(ColorDimStepsId, shaded);
            _material.SetColor(ColorDimCurveId, shaded);
        }

        if (driveRimColor)
            _material.SetColor(RimColorId, color);

        if (driveGradientColor)
            _material.SetColor(GradientColorId, color);
    }

    private void RestoreOriginalColors()
    {
        if (_material == null || _originalColors == null) return;

        for (int i = 0; i < AllColorIds.Length; i++)
            if (_material.HasProperty(AllColorIds[i]))
                _material.SetColor(AllColorIds[i], _originalColors[i]);
    }
}