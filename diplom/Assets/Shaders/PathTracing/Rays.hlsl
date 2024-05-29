#pragma once

#include "Assets/Shaders/Scene/Camera.hlsl"

struct Ray
{
    float3 Origin;
    float3 Direction;
};

Ray RayFrom(
    const float3 origin,
    const float3 direction)
{
    Ray ray;
    ray.Origin = origin;
    ray.Direction = direction;
    return ray;
}

Ray BuildInitialRay(
    const float2 uv,
    const CameraData camera)
{
    const float halfFov = camera.Fov * .5;
    const float distanceToScreen = cos(halfFov) / sin(halfFov);
    const float3 base = camera.Orientation.Forward * distanceToScreen;
    const float3 offset = camera.Orientation.Right * camera.AspectRatio * (uv.x - .5) * 2 + camera.Orientation.Up * (uv.y - .5) * 2;
    return RayFrom(camera.WorldPosition, normalize(base + offset));
}

Ray BuildInitialRay(const float2 uv)
{
    return BuildInitialRay(uv, ProvideCameraData());
}