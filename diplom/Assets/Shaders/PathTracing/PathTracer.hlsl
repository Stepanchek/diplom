#pragma once

#include "RayTracingGeometryStructure.hlsl"
#include "GeometryTracing.hlsl"
#include "Assets/Shaders/Scene/Sky.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
#include "SkySampling.hlsl"
#include "BRDF.hlsl"

#define MAX_BOUNCE_COUNT 32
#define ERROR float3(1, 0, 1)

void UpdateHash(inout uint hash)
{
    hash = JenkinsHash(hash);
}

void InitializeHash(const in float2 uv, out uint hash)
{
    Hash_Tchou_2_1_uint(asuint(uv), hash);
}

struct HitCacheData
{
    float3 Point;
    float3 Ray;
    float3 Normal;
    MaterialData Material;
    bool IsSpecular;
};

HitCacheData HitCacheFrom(
    const float3 hitPoint,
    const float3 ray,
    const float3 normal,
    const MaterialData material,
    const bool isSpecular)
{
    HitCacheData data;
    data.Point = hitPoint;
    data.Ray = ray;
    data.Normal = normal;
    data.Material = material;
    data.IsSpecular = isSpecular;
    return data;
}

bool RollReflectionChance(
    const uint seed,
    const float3 view,
    const float3 normal,
    const float indexOfRefraction)
{
    const float fresnel = F_Schlick(dot(view, normal), indexOfRefraction);

    return RollChance(fresnel, seed);
}

half3 PathTraceScene(
    const in float2 uv,
    const in RayTracingGeometryStructure geometry,
    const in SkyData sky,
    const in CameraData cameraData,
    const in uint randOffset)
{
    uint hash;
    InitializeHash(uv, hash);
    hash += randOffset;

    Ray ray = BuildInitialRay(uv, cameraData);
    HitCacheData HitCache[MAX_BOUNCE_COUNT];

    int depth;
    for(depth = 0; depth <= MAX_BOUNCE_COUNT; depth++)
    {
        HitData hit;
        MaterialData material;
        
        if(!TryRaycastGeometry(ray, geometry, hit, material))
            break;
        
        const float3 view = -ray.Direction;
        const float3 normal = hit.Normal;
        float3 light;
        bool isSpecular;

        UpdateHash(hash);
        if(RollReflectionChance(hash, view, normal, material.IndexOfRefraction))
        {
            UpdateHash(hash);
            light = SampleHemisphereCookTorrance(hash, view, normal, material.Roughness);
            const float lDotN = dot(light, normal);
            
            if(lDotN < 0)
            {
                light -= 2 * lDotN * normal;
            }
            
            isSpecular = true;
        }
        else
        {
            UpdateHash(hash);
            light = SampleHemisphereCosine(hash, normal);
            isSpecular = false;
        }

        HitCache[depth] = HitCacheFrom(hit.Point, ray.Direction, hit.Normal, material, isSpecular);
        ray = RayFrom(hit.Point + normal * 0.001, light);
        UpdateHash(hash);
    }
    
    if(depth > MAX_BOUNCE_COUNT)
    {
        return float3(0, 0, 0);
    }

    float3 light = SampleSky(sky, ray.Direction);
    float3 lightDirection = ray.Direction;

    for(int i = depth - 1; i >= 0; i--)
    {
        if(i < 0 || i > MAX_BOUNCE_COUNT)
            return ERROR;
        
        const HitCacheData currentHit = HitCache[i];
        const float3 viewDirection = -currentHit.Ray;
        const MaterialData material = currentHit.Material;

        if(currentHit.IsSpecular)
        {
            light *= EvaluateSampleWeightCookTorrance(viewDirection, lightDirection, currentHit.Normal, material);
        }
        else
        {
            light *= OrenNayarNoPiNoCos(
                viewDirection,
                lightDirection,
                currentHit.Normal,
                material.Albedo,
                material.Roughness);
        }

        light += material.Emission;
        lightDirection = currentHit.Ray;
    }
    
    return light;
}

