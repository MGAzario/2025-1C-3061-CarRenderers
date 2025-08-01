
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
       
            
            float3 A = tex2D(textureSampler, input.TextureCoordinates).rgb;
                          
            float3 N = normalize(input.Normal.xyz);
            float3 L = normalize(lightPosition - input.WorldPosition.xyz);
            float3 V = normalize(eyePosition - input.WorldPosition.xyz);
            float3 H = normalize(L + V);
            
            // Lighting terms
            float3 ambientTerm  = ambientColor * KAmbient;
            float  NdotL        = max(dot(N, L), 0);
            float3 diffuseTerm  = diffuseColor * (KDiffuse * NdotL);
            float  NdotH        = max(dot(N, H), 0);
            float3 specularTerm = specularColor * (KSpecular * pow(NdotH, shininess));
            
            // Compose
            float3 lit   = ambientTerm + diffuseTerm + specularTerm;
            float3 color = saturate(lit * A); // ensure not out-of-range
            
            return float4(color, 1);
            
           
             
}


technique BasicColorDrawing
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};


