
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

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

Texture2D    baseTexture;
sampler2D    textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU  = Wrap;      
    AddressV  = Wrap;
};

Texture2D    normalMap;
sampler2D    normalSampler = sampler_state
{
    Texture   = (normalMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};


Texture2D    specularMap;
sampler2D    specularSampler = sampler_state
{
    Texture   = (specularMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};



struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0; 
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;    
    float4 Color : COLOR0; 
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, World);
    output.Normal = mul(input.Normal, InverseTransposeWorld);
    output.TextureCoordinates = input.TextureCoordinates;
    output.Color = input.Color;
	
	return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    // 1) Sample base color including alpha
    float4 texColor = tex2D(textureSampler, input.TextureCoordinates);

    // Discard nearly transparent pixels (helps clean up leaf edges)
    if (texColor.a < 0.1f)
        discard;

    float3 A = texColor.rgb;

    // 2) Normal-map bump
    float3 nm = tex2D(normalSampler, input.TextureCoordinates).xyz * 2 - 1;  
    float3 N  = normalize(mul(nm, (float3x3)InverseTransposeWorld));

    // 3) Compute lighting directions
    float3 L = normalize(lightPosition   - input.WorldPosition.xyz);
    float3 V = normalize(eyePosition     - input.WorldPosition.xyz);
    float3 H = normalize(L + V);

    // 4) Ambient + Diffuse
    float3 ambientTerm = ambientColor * KAmbient;
    float  NdotL       = max(dot(N, L), 0);
    float3 diffuseTerm = diffuseColor * (KDiffuse * NdotL);

    // 5) Specular with specular-map factor
    float  NdotH        = max(dot(N, H), 0);
    float  specFactor   = tex2D(specularSampler, input.TextureCoordinates).r;
    float3 specularTerm = specularColor * (KSpecular * specFactor * pow(NdotH, shininess));

    // 6) Final color composition
    float3 lit   = ambientTerm + diffuseTerm + specularTerm;
    float3 color = lit * A;

    return float4(color, texColor.a);
}


technique BasicColorDrawing
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};


