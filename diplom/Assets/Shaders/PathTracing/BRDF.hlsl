#pragma once

#include "Assets/Shaders/Math/Math.hlsl"
#include "MaterialData.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"
#include "UnityCG.cginc"

void RandomPolarCosine(
    const uint seed,
    out float theta,
    out float phi)
{
    const float firstRandomValue = GenerateHashedRandomFloat(seed);
    const float secondRandomValue = GenerateHashedRandomFloat(seed + 24905678);
    theta = acos(sqrt(firstRandomValue));
    phi = secondRandomValue * UNITY_TWO_PI;
}

float3 SampleHemisphereCosine(
    const uint seed,
    const float3 normal)
{
    float theta, phi;
    RandomPolarCosine(seed, theta, phi);
    const float3 sample = SphericalToCartesian(theta, phi);
    return RotateHemisphereSample(FLOAT3_UP, normal, sample);
}

float3 OrenNayarNoPiNoCos(
    const float3 view,
    const float3 light,
    const float3 normal,
    const float3 albedo,
    const float roughness)
{
    const float correctedRoughness = roughness * .33;
    const float normalDeviation = Square(correctedRoughness);
    const float cosThetaI = saturate(dot(light, normal));
    const float cosThetaR = saturate(dot(view, normal));
    const float thetaI = acos(cosThetaI);
    const float thetaR = acos(cosThetaR);
    const float alpha = max(thetaI, thetaR);
    const float beta = min(thetaI, thetaR);
    const float3 viewProjection = ProjectVectorOntoSurface(normal, view);
    const float3 lightingProjection = ProjectVectorOntoSurface(normal, light);
    const float azimuthDifferenceCos = dot(normalize(viewProjection), normalize(lightingProjection));
    const float c1 = 1.0 - float(0.5) * normalDeviation / (normalDeviation + float(0.33));
    const float c2 = azimuthDifferenceCos >= 0
                ? 0.45 * normalDeviation / (normalDeviation + 0.09) * sin(alpha)
                : 0.45 * normalDeviation / (normalDeviation + 0.09) * (sin(alpha) - Cube(2 * beta * UNITY_INV_PI));
    const float c3 = 0.125 * normalDeviation / (normalDeviation + 0.09) * Square(4 * alpha * beta * Square(UNITY_INV_PI));
    const float3 l1 = albedo * (c1 + c2 * azimuthDifferenceCos * tan(beta) + c3 * (1 - abs(azimuthDifferenceCos)) * tan((alpha + beta) * .5));
    const float3 l2 = 0.17 * Square(albedo) * normalDeviation / (normalDeviation + 0.13) * (1 - azimuthDifferenceCos * Square(2 * beta * UNITY_INV_PI));

    return l1 + l2;
}

float3 OrenNayarNoCos(
    const float3 view,
    const float3 lighting,
    const float3 normal,
    const float3 albedo,
    const float roughness)
{
    return OrenNayarNoPiNoCos(view, lighting, normal, albedo, roughness) * UNITY_INV_PI;
}

float F_Schlick(
    const float nDotV,
    const float indexOfRefraction)
{
    const float r0 = Square((indexOfRefraction - 1) * (indexOfRefraction + 1));
    const float x = 1 - nDotV;
    const float x2 = x * x;
    const float x5 = x2 * x2 * x;
    return r0 + (1 - r0) * x5;
}

float MicrofacetDistributionGgx(
    const float mDotN,
    const float thetaM,
    const float width)
{
    const float cosTheta = cos(thetaM);
    const float cosTheta2 = Square(cosTheta);
    const float alpha = width * width;
    
    return alpha * (mDotN > 0 ? 1 : 0) / (UNITY_PI * Square(cosTheta2) * Square(alpha + Square(tan(thetaM))));
}

void SampleMicrofacetGgx(
    const uint seed,
    const float width,
    out float theta,
    out float phi)
{
    const float firstRandomValue = GenerateHashedRandomFloat(seed);
    const float secondRandomValue = GenerateHashedRandomFloat(seed + 3489257);
    theta = atan(width * sqrt(firstRandomValue) / sqrt(1 - firstRandomValue));
    phi = UNITY_TWO_PI * secondRandomValue;
}

float G1(
    const float3 vDotM,
    const float vDotN,
    const float thetaV,
    const float width)
{
    return (vDotM / vDotN) > 0
        ? (2.0 / (1 + sqrt(1 + Square(width) * Square(tan(thetaV)))))
        : 0;
}

float ShadowingSmith(
    const float vDotM,
    const float vDotN,
    const float lDotN,
    const float thetaO,
    const float roughness)
{
    const float ggx2 = G1(vDotM, vDotN, thetaO, roughness);
    const float ggx1 = G1(vDotM, lDotN, thetaO, roughness);

    return ggx1 * ggx2;
}

float3 MicrofacetToOutward(
    const float3 microfacet,
    const float3 view)
{
    return 2 * dot(microfacet, view) * microfacet - view;
}

float3 SampleHemisphereCookTorrance(const uint seed, const float3 view, const float3 normal, const float width)
{
    float theta, phi;
    SampleMicrofacetGgx(seed, width, theta, phi);
    const float3 microfacetSample = SphericalToCartesian(theta, phi);
    const float3 microfacetNormal = RotateHemisphereSample(FLOAT3_UP, normal, microfacetSample);

    return MicrofacetToOutward(microfacetNormal, view);
}

float EvaluateSampleWeightCookTorrance(
    const float3 view,
    const float3 light,
    const float3 normal,
    const MaterialData material)
{
    const float3 microfacet = normalize(view + light);
    const float nDotV = dot(normal, view);
    const float nDotL = dot(normal, light);
    const float nDotM = dot(normal, microfacet);
    const float vDotM = dot(view, microfacet);
    
    return vDotM * ShadowingSmith(vDotM, nDotV, nDotL, acos(vDotM), material.Roughness) / (vDotM * nDotM);
}