Shader "Custom/PathTracingRenderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            sampler2D _MainTex;
            #include "Fragment.hlsl"
            
            ENDHLSL
        }
    }
}
