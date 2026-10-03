using UnityEngine;

public static class ColorUtilities
{
    public const int BANDCOUNT = 7;

    public const float BANDWIDTH = .05f;
    public const float FADEWIDTH = .05f;
    
    public static Color FloatToColor(float f)
    {
        const int n = BANDCOUNT;

        float x = Mathf.Repeat(f, 1f) * n - 0.5f;
        int i0 = Mathf.FloorToInt(x);
        float frac = x - i0;

        int a = ((i0 % n) + n) % n;
        int b = (a + 1) % n;

        return Color.Lerp(((SpectrumColor)a).ToColor(), ((SpectrumColor)b).ToColor(), frac);
    }

    public static float Visibility(float dial, float center)
    {
        float d = Mathf.Abs(dial - center);
        d = Mathf.Min(d, 1f - d);

        return 1f - Mathf.Clamp01((d - BANDWIDTH) / FADEWIDTH);
    }
}
