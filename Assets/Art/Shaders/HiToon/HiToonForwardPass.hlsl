TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

#ifdef _ENABLE_NORMAL_MAP
TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);
#endif


#ifdef _ENABLE_EMISSION
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);
#endif

CBUFFER_START(UnityPerMaterial)
half4 _BaseColor;
half4 _RimColor;
half4 _ShadeColorOverride;
half4 _HeightGradientStartColor;
half4 _EmissionColor;


float4 _BaseMap_ST;
#ifdef _ENABLE_EMISSION
float4 _EmissionMap_ST;
#endif
float _LightAffection;

float _ShadeDistance;
float _Shade;
float _ShadeSmoothness;

float _Rim;
float _RimMultiplier;

float _HeightGradientStartPosition;
float _HeightGradientEndPosition;


float _Smoothness;
float _SpecularSize;
half4 _SpecularColor;

float _BacklightOffset;
float4 _BacklightColor;


CBUFFER_END

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    float4 tangentOS : TANGENT;
    half3 normalOS : NORMAL;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS: TEXCOORD1;
    half4 shadowCoord: TEXCOORD2;
    float fogFactor : TEXCOORD3;
    float4 positionOS : TEXCOORD4;

    half3 normal :TEXCOORD5;
    half3 view: TEXCOORD6;
    #ifdef _ENABLE_NORMAL_MAP
    half3 bitangent : TEXCOORD7;
    half3 tangent : TEXCOORD8;
    #endif


    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

float backlight(Light light, float3 normal)
{
    float backlight = dot(-light.direction, normal);
    backlight = (backlight + 1) * .5;
    backlight = clamp(backlight + _BacklightOffset, 0.0, 1.0);
    return backlight;
}

float calculateRim(float3 view, float3 normal)
{
    float rim = 1. - abs(dot(view, normal));
    return rim;
}

float specular(float3 light, float3 view, float3 normal, float smoothness, float size)
{
    float3 halfVector = normalize(light + view);
    float NdotH = dot(normal, halfVector) * 0.5 + 0.5;
    float specularSize = size; //UNITY_ACCESS_INSTANCED_PROP(Props, size);
    float specEdgeSmooth = smoothness; //UNITY_ACCESS_INSTANCED_PROP(Props, smoothness);
    float specular = saturate(pow(NdotH, 100.0 * (1.0 - specularSize) * (1.0 - specularSize)));
    float specularTransition = smoothstep(0.5 - specEdgeSmooth * 0.1, 0.5 + specEdgeSmooth * 0.1, specular);

    return specularTransition;
}


float inverseLerp(float A, float B, float T)
{
    return (T - A) / (B - A);
}

half3 applyFog(float3 defaultColor, float3 worldPosition)
{
    float fogFactor = ComputeFogFactor(worldPosition.z);
    return MixFog(defaultColor, fogFactor);
}

// The vertex shader definition with properties defined in the Varyings 
// structure. The type of the vert function must match the type (struct)
// that it returns.
Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionOS = IN.positionOS;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
    
    half3 view = SafeNormalize(GetCameraPositionWS() - OUT.positionWS);

    //GetWorldSpaceViewDir(OUT.positionWS);

    OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
    OUT.fogFactor = ComputeFogFactor(OUT.positionHCS.z);



    
    OUT.normal = TransformObjectToWorldNormal(IN.normalOS);
    OUT.view = view;

    #ifdef _ENABLE_NORMAL_MAP
    real sign = IN.tangentOS.w * GetOddNegativeScale();

    OUT.tangent = TransformObjectToWorldNormal(IN.tangentOS);
    half3 bitangent = cross(OUT.normal, OUT.tangent) * sign;
    OUT.bitangent = bitangent;
    #endif

    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

    IN.normal = NormalizeNormalPerPixel(IN.normal);

    #ifdef _ENABLE_NORMAL_MAP
    half4 n = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv);
    half3 normalTS = UnpackNormal(n);
    IN.normal = TransformTangentToWorld(normalTS,
half3x3(IN.tangent.xyz, IN.bitangent.xyz, IN.normal.xyz));
    #endif

    half3 additionalLightColor = half3(0, 0, 0);
    for (int i = 0; i < GetAdditionalLightsCount(); i++)
    {
        Light additionalLight = GetAdditionalLight(i, IN.positionWS);
        additionalLightColor += additionalLight.color * additionalLight.distanceAttenuation;
    }


    Light mainLight = GetMainLight(IN.shadowCoord);

    float lambert = max(0, dot(IN.normal, mainLight.direction) - _ShadeDistance);
    float smoothLambert = max(0, smoothstep(0, _ShadeSmoothness * .5,
                                            lambert));

    float3 color = _BaseColor.xyz * tex.xyz;

    color *= (mainLight.color + additionalLightColor) * _LightAffection;

    half3 ambient = (1. - smoothLambert) * color;


    half3 ambientColor = color;
    #ifdef _OVERRIDE_SHADE_COLOR
    ambient *= _ShadeColorOverride;
    ambientColor *= _ShadeColorOverride;
    #else
    ambient *= _Shade;
    ambientColor *= _Shade;
    #endif

    color = (min(1, color)) * smoothLambert + ambient;


    #ifdef _ENABLE_RIM
    float rim = calculateRim(IN.view, IN.normal) * _RimMultiplier;
    rim = pow(rim, _Rim);
    color = lerp(color, _RimColor, rim);
    #endif

    #ifdef _ENABLE_SPECULAR
    float spec = specular(mainLight.direction, IN.view, IN.normal, _Smoothness, _SpecularSize);
    color = lerp(color, _SpecularColor, spec * smoothLambert);
    #endif


    // #ifdef _ENABLE_RECEIVED_SHADOWS
    //     color = lerp(
    //         lerp(color, half4(0,0,0,0), 1),
    //         color,
    //         light.shadowAttenuation
    //     );
    //
    // #endif


    #ifdef _ENABLE_RECEIVED_SHADOWS
    color = lerp(color, ambientColor, 1. - mainLight.shadowAttenuation);
    #endif

    #ifdef _ENABLE_BACKLIGHT
    float backl = backlight(mainLight, IN.normal) * _BacklightColor.a;
    color = lerp(color, _BacklightColor, backl);
    #endif

    //color = lerp(color, half4(0,0,0,0), 1. - atten);

    #ifdef _ENABLE_HEIGHT_GRADIENT
    #ifdef _HEIGHTGRADIENTTYPE_WORLDSPACE
    float t = 1. - inverseLerp(_HeightGradientStartPosition, _HeightGradientEndPosition, IN.positionWS.y);
    #else
    float t = 1. - inverseLerp(_HeightGradientStartPosition, _HeightGradientEndPosition, IN.positionOS.y);
    #endif
    t = clamp(t, 0, 1);
    half3 targetColor = _HeightGradientStartColor; // * smoothLambert + ambient;
    color = lerp(color, targetColor, t * _HeightGradientStartColor.a);
    #endif

    #ifdef _ENABLE_EMISSION
    half emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.uv).r;
    color += _EmissionColor * emissionMap * smoothLambert;
    #endif


    color = MixFog(color, IN.fogFactor.x);

    return half4(color, 1);
}