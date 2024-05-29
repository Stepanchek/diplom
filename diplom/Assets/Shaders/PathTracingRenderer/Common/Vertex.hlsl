#pragma once

#include "UnityCG.cginc"

struct Appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct V2F
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

V2F Vert (const Appdata v)
{
    V2F o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}