#ifndef BLINNPHONGMATH_HLSL     // if not already defined…
#define BLINNPHONGMATH_HLSL


float4 BlinnPhongPSMath(
    float3 normal,
    float3 worldPos,
    float3 lightPos,
    float3 eyePos,
    float3 specularColor,
    float shininess,
    float KSpecular,
    float KDiffuse,
    float3 diffuseColor,
    float3 ambientColor,
    float KAmbient,
    float2 uv,
    float3 headLight
    )
{
    float3 V = normalize(eyePos   - worldPos); // the camera, the source of camera.
    float4 texelColor = SceneTex.Sample(SceneSamp, uv);
    float3 ambientLightFinal = KAmbient * ambientColor;
    normal = normalize(normal);
    
    float3 L = normalize(lightPos - worldPos); // the light, the source of light.
    float3 H = normalize(L + V);                //The half vector.
    float  NdotH = saturate(dot(normal, H));  // saturate (ndoth)
    float3 spec = specularColor * pow(NdotH, shininess);/// how shinny is the material.
    float NdotL = saturate(dot(normal, L));
    float specularLightFinal = NdotH * spec * KSpecular;//specular light * saturate(n dot h)
    float3 diffuseLightFinal = KDiffuse * diffuseColor * NdotL;

    /*float3 Lh = normalize(headLight - worldPos);
    float3 Hh = normalize(Lh + V);
    float  NdotHh = saturate(dot(normal, Hh));
    float3 specH = specularColor * pow(NdotHh, shininess);
    float NdotLh = saturate(dot(normal, Lh));
    float specularHeadLightF = NdotHh * specH * KSpecular;
    float3 diffuseHeadLightF = KDiffuse * diffuseColor * NdotLh;*/

    float3 totalLightingAux = ambientLightFinal + diffuseLightFinal  /*+ diffuseHeadLightF*/;

    float3 totalSpecularLight = specularLightFinal /*+ specularHeadLightF*/;
    
    return float4((totalLightingAux * texelColor.rgb) + totalSpecularLight,texelColor.a);


    //////////////////
    
}
/*
 *Blinn Phong math.
H = normalize (l+v)
color = ks * specular light * saturate(n dot h)

color final = (ambient light + diffuse light) * textcolor + specular light, a.
*/

#endif
