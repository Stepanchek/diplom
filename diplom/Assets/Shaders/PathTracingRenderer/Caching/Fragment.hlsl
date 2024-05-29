#pragma once
#include <HLSLSupport.cginc>

cbuffer InputBuffer : b1
{
    int CacheLength;
    sampler2D SampleCache;
    sampler2D NewSample;
}

fixed4 Frag (const V2F i) : SV_Target
{
    const int newCacheSize = CacheLength + 1;
    return CacheLength / (float)newCacheSize * tex2D(SampleCache, i.uv) + tex2D(NewSample, i.uv) / newCacheSize;
}
