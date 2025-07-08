#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D    SceneTex    : register(t0);
SamplerState SceneSamp   : register(s0);
Texture2D    BloomTex    : register(t1);
SamplerState BloomSamp   : register(s1);
Texture2D    EnvTex      : register(t2);
SamplerState EnvSmp       : register(s2);



float4x4 WorldViewProjection;
float4x4 World;
float4x4 InverseTransposeWorld;

float3 ambientColor; // Light's Ambient Color
float3 diffuseColor; // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient; 
float KDiffuse; 
float KSpecular;
float shininess; 
float3 lightPosition;
float3 eyePosition; // Camera position
float3 headLight; // a bit.... what to do with this.


#include "BlinnPhongMath.hlsl"
#include "EnvHelper.hlsl"
#include "BloomHelper.hlsl"

/*
texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture envMap;
samplerCUBE envMapSampler = sampler_state
{
    Texture = envMap;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture bloomTexture;
sampler2D bloomTextureSampler = sampler_state
{
    Texture = (bloomTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};*/

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;    
};



VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    float3 worldNormal = mul(float4(input.Normal.xyz, 0.0), InverseTransposeWorld).xyz;
    worldNormal = normalize(worldNormal);
    output.Normal = float4(worldNormal, 0.0);
    output.TextureCoordinates = input.TextureCoordinates;
    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, World);
    
    return output;
}

float4 CarPS(VertexShaderOutput input) : SV_TARGET
{
    
    float4 blinnPhong = BlinnPhongPSMath(input.Normal.xyz,
                                           input.WorldPosition.xyz,
                                           lightPosition,
                                           eyePosition,
                                           specularColor,
                                           shininess,
                                           KSpecular,
                                           KDiffuse,
                                           diffuseColor,
                                           ambientColor,
                                           KAmbient,
                                           input.TextureCoordinates,
                                           headLight);
                                           
    float3 env = EnvEffectMath(
            input.TextureCoordinates,
            input.Normal.xyz,
            eyePosition,
            input.WorldPosition.xyz
        ).rgb * 0.2;
    
        // Combine lit‑and‑textured + env
        float3 final = saturate(blinnPhong.rgb) + env;
        return float4(final, blinnPhong.a);
}

float4 BloomExtractPS(VertexShaderOutput input) : SV_TARGET
{
    return ExtractCyanBloom(input.TextureCoordinates);
}

float4 BloomCompositePS(VertexShaderOutput input) : SV_TARGET
{
    return CompositeCyanBloom(input.TextureCoordinates);
}

technique CarShader
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL CarPS();
    }
}

technique BloomExtract
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL BloomExtractPS();
    }
}

technique BloomComposite
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL BloomCompositePS();
    }
}

