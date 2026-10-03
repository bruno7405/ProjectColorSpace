using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class ColorUtilities
{
    public const int BANDCOUNT = 7;

    public const float BANDWIDTH = .05f;
    public const float FADEWIDTH = .05f;

    public const float GLOBAL_SATURATION = 1f;
    public const float GLOBAL_VALUE = 0.9f;
    
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

    public static Color HueToRBG(float hue)
    {
        float hueSector = hue * 6;
        float secondaryChroma = GLOBAL_SATURATION * GLOBAL_VALUE * (1 - Mathf.Abs((hueSector % 2) - 1));
        Vector3 rgbColor = Vector3.zero;

        if (hueSector < 1)
        {
            rgbColor = new Vector3(1, secondaryChroma, 0);
        }
        else if (hueSector < 2)
        {
            rgbColor = new Vector3(secondaryChroma, 1, 0);
        }
        else if (hueSector < 3)
        {
            rgbColor = new Vector3(0, 1, secondaryChroma);
        }
        else if (hueSector < 4)
        {
            rgbColor = new Vector3(0, secondaryChroma, 1);
        }
        else if (hueSector < 5)
        {
            rgbColor = new Vector3(secondaryChroma, 0, 1);
        }
        else
        {
            rgbColor = new Vector3(1, 0, secondaryChroma);
        }

        float m = GLOBAL_VALUE - (GLOBAL_VALUE * GLOBAL_SATURATION);
        rgbColor = new Vector3(rgbColor.x + m, rgbColor.y + m, rgbColor.z + m);

        return new Color(rgbColor.x, rgbColor.y, rgbColor.z);
    }

    public static float RGBtoHue(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;
    }

    public static float Visibility(float dial, float center)
    {
        float d = Mathf.Abs(dial - center);
        d = Mathf.Min(d, 1f - d);

        return 1f - Mathf.Clamp01((d - BANDWIDTH) / FADEWIDTH);
    }
}
