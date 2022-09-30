// This shader fills the mesh shape with a color predefined in the code.
Shader "HiToon/Toon"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _BacklightColor("Backlight Color", Color) = (1,1,1,1)
        _ShadeColorOverride("Shade Color Override", Color) = (1,1,1,1)
        _HeightGradientStartColor("Height Gradient Start Color", Color) = (1,1,1,1)
        [HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)

        _BaseMap("BaseMap", 2D) = "white"
        _NormalMap("NormalMap", 2D) = "bump"
        _EmissionMap("EmissionMap", 2D) = "white"


        _LightAffection("Light Affection", Float) = 1
        _ShadeDistance("Shade Distance", Float) = 0
        _ShadeSmoothness("Shade Smoothness", Float) = 0.05
        _Shade("Shade", Float) = 0.6
        _SpecularSize("Specular size",Float) = 0
        _Smoothness("Smoothness", Float) = .2
        _Rim("Rim", Float) = 0
        _RimMultiplier("Rim Multiplier",Float) = 1
        _BacklightOffset("Backlight Offset", Float) = 0

        _HeightGradientStartPosition("Height Gradient Start Position",Float) = 0
        _HeightGradientEndPosition("Height Gradient End Position",Float) = 0


        // shader features
        [Toggle(_ENABLE_RIM)] _RimEnabled("Enable Rim", Int) = 0
        [Toggle(_ENABLE_BACKLIGHT)] _BacklightEnabled("Enable Backlight", Int) = 0
        [Toggle(_ENABLE_SPECULAR)] _SpecularEnabled("Enable Specular", Int) = 0
        [Toggle(_OVERRIDE_SHADE_COLOR)] _OverrideShadeColor("Override Shade Color", Int) = 0
        [Toggle(_ENABLE_EXTRA_LAYER)] _ExtraLayerEnabled("Enable Extra Layer", Int) = 0
        [Toggle(_ENABLE_RECEIVED_SHADOWS)] _ReceivedShadowsEnabled("Enable Received Shadows", Int) = 0
        [Toggle(_ENABLE_HEIGHT_GRADIENT)] _HeightGradientEnabled("Enable Height Gradient", Int) = 0
        [KeywordEnum(WorldSpace, ObjectSpace)] _HeightGradientType ("Height Gradient Type", Int) = 0
        [Toggle(_ENABLE_EMISSION)] _EmissionEnabled("Enable Emission", Int) = 0
        [Toggle(_ENABLE_NORMAL_MAP)] _NormalMapEnabled("Enable Normal Map", Int) = 0



        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"
        }

        LOD 300

        Pass
        {
            Name "Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }


            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _ENABLE_BACKLIGHT
            #pragma shader_feature _ENABLE_RIM
            #pragma shader_feature _ENABLE_SPECULAR
            #pragma shader_feature _OVERRIDE_SHADE_COLOR
            #pragma shader_feature _ENABLE_RECEIVED_SHADOWS
            #pragma shader_feature _ENABLE_HEIGHT_GRADIENT
            #pragma shader_feature _ENABLE_EMISSION
            #pragma shader_feature _ENABLE_NORMAL_MAP

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _HEIGHTGRADIENTTYPE_WORLDSPACE _HEIGHTGRADIENTTYPE_OBJECTSPACE

            #pragma multi_compile_fog

            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "HiToonForwardPass.hlsl"
            
            ENDHLSL

        }
        UsePass "Universal Render Pipeline/Simple Lit/DepthNormals"
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Depth Only"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        }
    }

    CustomEditor "HiToonEditor"
}