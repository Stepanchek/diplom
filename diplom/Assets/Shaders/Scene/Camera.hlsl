#pragma once

#include <HLSLSupport.cginc>

CBUFFER_START(Camera)
    float3 Camera_Up;
    float3 Camera_Forward;
    float3 Camera_Right;
    float Camera_AspectRatio;
    float Camera_Fov;
    float3 Camera_Position;
    uint2 Screen_Resolution;
CBUFFER_END


struct Orientation
{
    float3 Forward;
    float3 Right;
    float3 Up;
};

Orientation OrientationFrom( 
    const float3 forward,
    const float3 right,
    const float3 up)
{
    Orientation orientation;
    orientation.Forward = forward;
    orientation.Right = right;
    orientation.Up = up;
    return orientation;
}

struct CameraData
{
    float3 WorldPosition;
    Orientation Orientation;
    float Fov;
    float AspectRatio;
};

CameraData CameraDataFrom(
    const float3 position,
    const Orientation orientation,
    const float fov,
    const float aspectRatio)
{
    CameraData data;
    data.WorldPosition = position;
    data.Orientation = orientation;
    data.Fov = fov;
    data.AspectRatio = aspectRatio;
    return data;
}

CameraData ProvideCameraData()
{
    return CameraDataFrom(
        Camera_Position,
        OrientationFrom(Camera_Forward, Camera_Right, Camera_Up),
        Camera_Fov,
        Camera_AspectRatio);
}
