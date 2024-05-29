#pragma once

#include "SphereData.hlsl"
#include "MaterialData.hlsl"

struct SphereData
{
    float3 Position; // World space
    float Radius; // World units
    MaterialData Material;
};

struct SphereCollection
{
    StructuredBuffer<SphereData> Items;
    int Count;
};