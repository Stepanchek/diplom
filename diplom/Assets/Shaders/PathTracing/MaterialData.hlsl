#pragma once

struct MaterialData
{
    float3 Albedo;
    float Roughness;
    float3 Emission;
    float IndexOfRefraction;
};

MaterialData MaterialDataFrom(
    const float3 albedo,
    const float roughness,
    const float3 emission,
    const float indexOfRefraction)
{
    MaterialData data;
    data.Albedo = albedo;
    data.Roughness = roughness;
    data.Emission = emission;
    data.IndexOfRefraction = indexOfRefraction;
    return data;
}