#pragma once

#include "Assets/Shaders/PathTracing/SphereData.hlsl"
#include "Assets/Shaders/PathTracing/Rays.hlsl"
#include "Assets/Shaders/PathTracing/HitData.hlsl"
#include "Assets/Shaders/Math/Math.hlsl"

bool TryRaycastSphere(
    const in Ray ray,
    const in SphereData sphere,
    out HitData hit)
{
    const float3 sphereRelativePosition = sphere.Position - ray.Origin;
    const float distanceToSphereCenter = length(sphereRelativePosition);
    const float raySphereCos = saturate(dot(ray.Direction, normalize(sphereRelativePosition)));
    const float adjacent = raySphereCos * distanceToSphereCenter;
    const float opposite = sqrt(Square(distanceToSphereCenter) - Square(adjacent));

    if(opposite > sphere.Radius)
    {
        hit = EmptyHit();
        return false;
    }

    const float internalAdjacent = sqrt(Square(sphere.Radius) - Square(opposite));
    const float distanceToPointOnSphere = adjacent - internalAdjacent;
    hit.Point = ray.Origin + ray.Direction * distanceToPointOnSphere;
    hit.Normal = normalize(hit.Point - sphere.Position);
    return true;
}

bool TryRaycastSpheres(
    const in Ray ray,
    const in SphereCollection spheres,
    out HitData hitInfo,
    out SphereData sphere)
{
    bool hasIntersection = false;
    HitData currentHit;
    
    for(int i = 0; i < spheres.Count; i++)
    {
        const SphereData currentSphere = spheres.Items[i];
        
        if(TryRaycastSphere(ray, currentSphere, currentHit))
        {
            if(hasIntersection)
            {
                if(length(currentHit.Point - ray.Origin) < length(hitInfo.Point - ray.Origin))
                {
                    hitInfo = currentHit;
                    sphere = currentSphere;
                }
            }
            else
            {
                hasIntersection = true;
                hitInfo = currentHit;
                sphere = currentSphere;
            }
        }
    }

    return hasIntersection;
}

bool TryRaycastFloor(
    const Ray ray,
    out HitData hit)
{
    const float3 floorNormal = FLOAT3_UP;
    const float vDotN = dot(floorNormal, ray.Direction);
    
    if(vDotN >= 0 || ray.Origin.y <= 0)
        return false;

    hit.Point = ray.Origin + ray.Direction * (ray.Origin.y / dot(ray.Direction, -floorNormal));
    hit.Normal = floorNormal;
    return true;
}

bool TryRaycastGeometry(
    const Ray ray,
    const RayTracingGeometryStructure geometry,
    out HitData hit,
    out MaterialData material)
{
    SphereData sphere;
    
    if(TryRaycastSpheres(ray, geometry.Spheres, hit, sphere))
    {
        material = sphere.Material;
        return true;
    }

    if(TryRaycastFloor(ray, hit))
    {
        material = geometry.FloorMaterial;
        return true;
    }

    return false;
}