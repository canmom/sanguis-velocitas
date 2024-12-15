float2 unity_gradientNoise_dir_float(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float periodicGradientNoise_float(float2 p, float scalex, float offsetx)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float periodicxplusone = ip.x + 1 >= scalex ? offsetx : ip.x + 1;
    float d00 = dot(unity_gradientNoise_dir_float(ip), fp);
    float d01 = dot(unity_gradientNoise_dir_float(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir_float(float2(periodicxplusone, ip.y)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir_float(float2(periodicxplusone, ip.y+1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

half2 unity_gradientNoise_dir_half(half2 p)
{
    p = p % 289;
    half x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(half2(x - floor(x + 0.5), abs(x) - 0.5));
}

half periodicGradientNoise_half(half2 p, half scalex)
{
    half2 ip = floor(p);
    half2 fp = frac(p);
    half periodicxplusone = ip.x + 1 >= scalex ? ip.x-scalex : ip.x + 1;
    half d00 = dot(unity_gradientNoise_dir_half(ip), fp);
    half d01 = dot(unity_gradientNoise_dir_half(ip + half2(0, 1)), fp - half2(0, 1));
    half d10 = dot(unity_gradientNoise_dir_half(half2(periodicxplusone, ip.y)), fp - half2(1, 0));
    half d11 = dot(unity_gradientNoise_dir_half(half2(periodicxplusone, ip.y+1)), fp - half2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

void GradientNoise_float(float2 UV, float2 Scale, float2 Offset, out float Out)
{
    Scale = floor(Scale); //force scale to be an integer or the periodicity won't work
    Out = periodicGradientNoise_float(UV * Scale + Offset, Scale.x + Offset.x, Offset.x) + 0.5;
}

void GradientNoise_half(half2 UV, half2 Scale, half2 Offset, out half Out)
{
    Scale = floor(Scale); //force scale to be an integer or the periodicity won't work
    Out = periodicGradientNoise_half(UV * Scale + Offset, Scale.x + Offset.x) + 0.5;
}