#pragma once

struct HitData
{
    float3 Point;
    float3 Normal;
};

HitData HitDataFrom(
    const float3 hitPoint,
    const float3 normal)
{
    HitData data;
    data.Point = hitPoint;
    data.Normal = normal;
    return data;
}

HitData EmptyHit()
{
    return HitDataFrom(0, 0);
}