#pragma once

#include <UnityCG.cginc>

#include "Assets/Shaders/Scene/Sky.hlsl"

float3 SampleSky(const in SkyData skyData, const in float3 viewDirection)
{
    const half4 skySampleRaw = skyData.Skybox.Sample(skyData.Sampler, viewDirection);
    const half3 skySample = DecodeHDR(skySampleRaw, skyData.DecodingInstructions);

    return skySample;
}