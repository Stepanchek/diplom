using PathTracingRendererModule.Extensions;
using PathTracingRendererModule.Materials;
using Unity.Mathematics;

namespace PathTracingRendererModule
{
    public readonly struct MaterialData
    {
        public readonly float3 Albedo;
        public readonly float Roughness;
        public readonly float3 Emission;
        public readonly float IndexOfRefraction;

        public MaterialData(
            float3 albedo,
            float roughness,
            float3 emission,
            float indexOfRefraction)
        {
            Albedo = albedo;
            Roughness = roughness;
            Emission = emission;
            IndexOfRefraction = indexOfRefraction;
        }

        public static MaterialData From(
            float3 albedo,
            float roughness,
            float3 emission,
            float indexOfRefraction)
        {
            return new(
                albedo,
                roughness,
                emission,
                indexOfRefraction);
        }

        public static MaterialData From(MaterialDataDescription descriptor)
        {
            return new(
                descriptor.Color.ToFloat().xyz,
                descriptor.Roughness,
                descriptor.Emission.ToFloat().xyz,
                descriptor.IndexOfRefraction);
        }
    }
}