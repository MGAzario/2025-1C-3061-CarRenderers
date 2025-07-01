#ifndef BLOOMHELPER_HLSL     // if not already defined…
#define BLOOMHELPER_HLSL


static const float3   BloomTargetColor   = float3(0.0, 0.78, 0.86);
static const float    BloomThreshold     = 0.15;
static const float    BloomCompositeFrac = 0.5;


float4 ExtractCyanBloom(float2 uv)
{
    float4  c        = SceneTex.Sample(SceneSamp, uv); // samples color from SceneSamp on UV
    float   distC    = distance(c.rgb, BloomTargetColor); // distance between BloomTarget and the UV rgb
    float   mask     = step(distC, BloomThreshold);// filter only the close ones.


    //We only get the brightest.
    return float4(c.rgb * mask, c.a);// return RGB
}

float4 CompositeCyanBloom(float2 uv)
{
    float4 scene = SceneTex.Sample(SceneSamp, uv); // sammple text
    float4 bloom = BloomTex.Sample(BloomSamp, uv); // Bloom pass reslut
    return scene * (1.0 - BloomCompositeFrac)
         + (scene * BloomCompositeFrac + bloom);
}

#endif