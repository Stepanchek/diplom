Shader "Custom/Path Tracing Cache Function"
{
    Properties
    {
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma use_dxc

            #include "../Common/Vertex.hlsl"
            #include "Fragment.hlsl"
            
            ENDHLSL
        }
    }
}
