
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
    AddressU = Clamp;
    AddressV = Clamp;
};


TextureCube    environmentMap    : register(t1);
samplerCUBE    environmentSampler: register(s1) = sampler_state
{
    Texture   = (environmentMap);
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
    AddressW  = Clamp;
};

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
    VertexShaderOutput output = (VertexShaderOutput) 0;
	output.Position = mul(input.Position, WorldViewProjection);
	output.WorldPosition = mul(input.Position, World);
    output.Normal = mul(float4(normalize(input.Normal.xyz), 1.0), InverseTransposeWorld);
    output.TextureCoordinates = input.TextureCoordinates;
	
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
        float3 A = tex2D(textureSampler, input.TextureCoordinates).rgb;

        float3 N = normalize(input.Normal.xyz);
        float3 L = normalize(lightPosition   - input.WorldPosition.xyz);
        float3 V = normalize(eyePosition     - input.WorldPosition.xyz);
        float3 H = normalize(L + V);

        float3 ambientTerm = ambientColor * KAmbient;

        float  NdotL      = max(dot(N, L), 0);
        float3 diffuseTerm = diffuseColor * (KDiffuse * NdotL);

        float  NdotH      = max(dot(N, H), 0);
        float3 specularTerm = specularColor * (KSpecular * pow(NdotH, shininess));
    
        float3 lit   = ambientTerm + diffuseTerm + specularTerm;
        
        //her comes the env shader after this. baseLitColor being the blinn phong diffuse.
        float3 baseLitColor = lit * A;
        
        float3 R         = reflect(-V, N);
        float3 envColor  = texCUBE(environmentSampler, R).rgb;
        
           
        float reflectionStrength = 0.12f;
        
        float3 finalColor = baseLitColor + envColor * reflectionStrength;
    
        return float4(finalColor, 1);
        
        
}

technique BasicColorDrawing
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};


