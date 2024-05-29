#pragma once
#include "Assets/Shaders/PathTracing/SphereData.hlsl"
#include "Assets/Shaders/PathTracing/RayTracingGeometryStructure.hlsl"

CBUFFER_START(GeometryGlobals)
    int SphereCount;
    float3 FloorAlbedo;
    float FloorRoughness;
    float3 FloorEmission;
    float FloorIndexOfRefraction;
CBUFFER_END

StructuredBuffer<SphereData> Spheres;

RayTracingGeometryStructure ProvideGeometryStructure()
{
    RayTracingGeometryStructure geometry;
    SphereCollection spheres;

    spheres.Items = Spheres;
    spheres.Count = SphereCount;
    geometry.Spheres = spheres;
    geometry.FloorMaterial = MaterialDataFrom(
        FloorAlbedo,
        FloorRoughness,
        FloorEmission,
        FloorIndexOfRefraction);

    return geometry;
}