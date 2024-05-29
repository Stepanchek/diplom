#pragma once
#include <HLSLSupport.cginc>

CBUFFER_START(Sky)
    UNITY_DECLARE_TEXCUBE(Skybox);
    float4 Skybox_HDR;
CBUFFER_END

struct SkyData
{
    TextureCube Skybox;
    SamplerState Sampler;
    float4 DecodingInstructions;
};

SkyData SkyDataFrom(
    const in TextureCube skybox,
    const in SamplerState skyboxSampler,
    const in float4 decodingInstructions)
{
    SkyData data;
    data.Skybox = skybox;
    data.Sampler = skyboxSampler;
    data.DecodingInstructions = decodingInstructions;
    return data;
}

SkyData ProvideSkyData()
{
    return SkyDataFrom(Skybox, samplerSkybox, Skybox_HDR);
}

