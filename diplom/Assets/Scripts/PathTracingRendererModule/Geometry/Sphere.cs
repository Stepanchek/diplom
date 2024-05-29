using PathTracingRendererModule.Materials;
using Unity.Mathematics;
using UnityEngine;

namespace PathTracingRendererModule.Geometry
{
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteAlways]
    public sealed class Sphere : MonoBehaviour
    {
        [SerializeField] private MaterialDataDescription _material;

        public SphereData ProvideData()
        {
            var cachedTransform = transform;
            
            return new(
                cachedTransform.position, 
                cachedTransform.lossyScale.x * .5f, 
                MaterialData.From(_material));
        }
    }

    public readonly struct SphereData
    {
        public readonly float3 Position;
        public readonly float Radius;
        public readonly MaterialData MaterialData;
        
        public SphereData(
            float3 position, 
            float radius, 
            MaterialData materialData)
        {
            Position = position;
            Radius = radius;
            MaterialData = materialData;
        }
    }
}