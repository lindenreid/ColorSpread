Shader "Custom/ColorGrowth"
{
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM

                #include "ColorGrowth.hlsl"

                #pragma vertex Vertex
                #pragma fragment Frag

            ENDHLSL
        }
    }
}