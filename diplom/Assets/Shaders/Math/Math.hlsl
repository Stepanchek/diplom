#pragma once

#define FLOAT3_UP float3(0, 1, 0) 

float Square(const float value)
{
    return value * value;
}

float Square(const float3 value)
{
    return value * value;
}

float Cube(const float value)
{
    return value * value * value;
}

float3 ProjectVectorOntoSurface(const float3 surfaceNormal, const float3 vec)
{
    const float3 direction = normalize(vec);
    const float cos = dot(surfaceNormal, direction);
    const float3 projectionOffset = -surfaceNormal * length(vec) * cos;
    return vec + projectionOffset;
}

float3 RotateAround(const float3 vec, const float3 axis, const float theta)
{
    float sinTheta, cosTheta;
    sincos(theta, sinTheta, cosTheta);

    return vec * cosTheta + cross(axis, vec) * sinTheta + axis * dot(axis, vec) * (1 - cosTheta);
}

float3 RotateHemisphereSample(
    const float3 from,
    const float3 to,
    const float3 hemisphereSample)
{
    if(length(from - to) < 0.001)
        return hemisphereSample;
    
    const float3 axis = normalize(cross(from, to));
    const float3 angle = acos(dot(from, to));
    
    return RotateAround(hemisphereSample, axis, angle);
}

// (0, 1, 0) is up
float3 SphericalToCartesian(const float theta, const float phi)
{
    float sinPhi, cosPhi, sinTheta, cosTheta;
    sincos(theta, sinTheta, cosTheta);
    sincos(phi, sinPhi, cosPhi);
    
    return float3(
               cosPhi * sinTheta,
               cosTheta,
               sinPhi * sinTheta);
}

bool RollChance(const float chance, const uint seed)
{
    return (float)(seed % 1000) / 1000 < chance;
}