Shader "FogOfWar/FogOfWarPlane"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
    #pragma multi_compile_instancing
 
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitProperties.hlsl"
 
    ENDHLSL

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="HDRenderPipeline" "RenderType" = "HDUnlitShader"}


        Pass {
            Stencil {
                Ref 4
                Comp NotEqual
                Pass Replace
                WriteMask 4
            }
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Off

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
 
            #pragma multi_compile _ DEBUG_DISPLAY
 
            #ifdef DEBUG_DISPLAY
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
            #endif
 
            #define SHADERPASS SHADERPASS_FORWARD_UNLIT
 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/ShaderPass/UnlitSharePass.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitData.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForwardUnlit.hlsl"

            float4 _Color;

            float4 Frag(PackedVaryingsToPS packedInput) : SV_Target
            {
                return _Color;
            }

            #pragma vertex Vert
            #pragma fragment Frag
 
            ENDHLSL
        }

    }
    FallBack "Diffuse"
}
