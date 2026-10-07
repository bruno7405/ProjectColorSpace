float2 VoronoiRandomVector(float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}

void VoronoiCenter_float(float2 UV, float AngleOffset, float CellDensity,
                         out float2 CellCenter, out float Dist, out float2 CellID)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float minDist = 8.0;
    CellCenter = 0;
    Dist = 0;
    CellID = 0;

    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 lattice = float2(x, y);
            float2 offset = VoronoiRandomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if (d < minDist)
            {
                minDist = d;
                Dist = d;
                CellCenter = (g + lattice + offset) / CellDensity;
                CellID = g + lattice;
            }
        }
    }
}

