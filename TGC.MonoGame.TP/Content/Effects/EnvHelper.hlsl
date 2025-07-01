#ifndef ENVHELPER_HLSL
#define ENVHELPER_HLSL



/*
float3 R = reflect(V, N);
float4 environmentColor = texCUBE(renderTarget, R);*/

float4 EnvEffectMath(float2 uv, float3 normal, float3 eyePos, float3 worldPos)
{
    float3 normalizedN = normalize(normal); //as name suggets.

    float3 baseColor = SceneTex.Sample(SceneSamp, uv).rgb; // get texel colors.

    baseColor = lerp(baseColor, float3(1,1,1), step(length(baseColor), 0.01));// here we save black texels. no black spot rendering.

    float3 cameraView = normalize(eyePos - worldPos); // the camera
    float3 reflection = reflect(cameraView, normalizedN);//reflection.
    float3 reflectionColor = texCUBE(EnvSmp, reflection).rgb;// The reflected image on the object.
    
    float fresnel = saturate((1.0 - dot(normal, cameraView)));// the reflected image curve surface.

    return float4(lerp(baseColor, reflectionColor, fresnel), 1.0f);//interpolation. of reflectioncolor + base color affected by fresnel.
    //lerp = x*(1-s) + y*s
    
}



#endif