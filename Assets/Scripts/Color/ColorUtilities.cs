using UnityEditor;
using UnityEngine;

public static class ColorUtilities
{
    public const int BANDCOUNT = 7;

    public const float BANDWIDTH = .05f;
    public const float FADEWIDTH = .05f;

    public const float GLOBAL_SATURATION = 1f;
    public const float GLOBAL_VALUE = 0.9f;

    public static readonly SpectrumColor[] Bands =
    {
      SpectrumColor.Red, SpectrumColor.Orange, SpectrumColor.Yellow,
      SpectrumColor.Green, SpectrumColor.Blue, SpectrumColor.Indigo,
      SpectrumColor.Violet  
    };
    
    public static Color FloatToColor(float f)
    {
        const int n = BANDCOUNT;

        float x = Mathf.Repeat(f * n - 0.5f, n);
        int i0 = Mathf.FloorToInt(x);
        float frac = x - i0;

        int a = i0 % n;
        int b = (a + 1) % n;

        return Color.Lerp(IndexToColor(a), IndexToColor(b), frac);
    }

    public static Color Shade(Color c, float saturation, float value)
    {
        Color result = Color.Lerp(Color.white, c, saturation) * value;
        result.a = 1f;
        return result;
    }

    static Color IndexToColor(int index)
    {
        return ((SpectrumColor)(1 << index)).ToColor();
    }

    public static Color HueToRBG(float hue, float saturation, float value)
    {
        float hueSector = hue * 6;
        float chroma = saturation * value;
        float secondaryChroma = chroma * (1 - Mathf.Abs((hueSector % 2) - 1));
        Vector3 rgbColor = Vector3.zero;

        if (hueSector < 1)
        {
            rgbColor = new Vector3(chroma, secondaryChroma, 0);
        }
        else if (hueSector < 2)
        {
            rgbColor = new Vector3(secondaryChroma, chroma, 0);
        }
        else if (hueSector < 3)
        {
            rgbColor = new Vector3(0, chroma, secondaryChroma);
        }
        else if (hueSector < 4)
        {
            rgbColor = new Vector3(0, secondaryChroma, chroma);
        }
        else if (hueSector < 5)
        {
            rgbColor = new Vector3(secondaryChroma, 0, chroma);
        }
        else
        {
            rgbColor = new Vector3(chroma, 0, secondaryChroma);
        }

        float m = value - (value * saturation);
        rgbColor = new Vector3(rgbColor.x + m, rgbColor.y + m, rgbColor.z + m);

        return new Color(rgbColor.x, rgbColor.y, rgbColor.z);
    }

    public static float RGBtoHue(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float colorMax = Mathf.Max(r, g, b);
        float colorMin = Mathf.Min(r, g, b);
        float colorDelta = colorMax - colorMin;

        float hue;

        if (colorMax == r)
        {
            hue = 60 * (((g - b) / colorDelta) % 6);
        }
        else if (colorMax == g)
        {
            hue = 60 * (((b - r) / colorDelta) + 2);
        }
        else
        {
            hue = 60 * (((r - g) / colorDelta) + 4);
        }

        hue = Mathf.Repeat(hue, 360f);
        return hue / 360f;
    }

    public static float Visibility(float dial, float center)
    {
        float d = Mathf.Abs(dial - center);
        d = Mathf.Min(d, 1f - d);

        return 1f - Mathf.Clamp01((d - BANDWIDTH) / FADEWIDTH);
    }

    public static float HueDifference(float to, float from)
    {
        return Mathf.Repeat(to - from + 0.5f, 1f) - 0.5f;
    }
}
