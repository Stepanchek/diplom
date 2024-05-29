#pragma once

#include "SphereData.hlsl"
#include "MaterialData.hlsl"

struct RayTracingGeometryStructure
{
    SphereCollection Spheres;
    MaterialData FloorMaterial;
};