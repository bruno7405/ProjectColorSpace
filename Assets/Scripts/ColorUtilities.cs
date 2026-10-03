using UnityEngine;

public static class ColorUtilities
{
    public static Color FloatToColor(float f)
    {
        const int n = SpectrumColorExtensions.BANDCOUNT;

        float x = Mathf.Repeat(f, 1f) * n - 0.5f;
        int i0 = Mathf.FloorToInt(x);
        float frac = x - i0;

        int a = ((i0 % n) + n) % n;
        int b = (a + 1) % n;

        return Color.Lerp(((SpectrumColor)a).ToColor(), ((SpectrumColor)b).ToColor(), frac);
    }
}
