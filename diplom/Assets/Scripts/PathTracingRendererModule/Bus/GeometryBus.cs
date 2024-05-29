using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PathTracingRendererModule.Extensions;
using PathTracingRendererModule.Geometry;
using PathTracingRendererModule.Materials;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace PathTracingRendererModule
{
    public interface IGeometryBus
    {
        void WriteTo(Material material);
    }
    
    public class GeometryBus : MonoBehaviour, IGeometryBus
    {
        [SerializeField] private List<Sphere> _spheresRegistry = new();
        [SerializeField] private MaterialDataDescription _floorMaterial;

        private ComputeBuffer _sphereBuffer;
        private ConstantBuffer<GeometryGlobals> _geometryGlobals;
        
        private static readonly int _spheres = Shader.PropertyToID("Spheres");
        private static readonly int _geometryBufferName = Shader.PropertyToID("GeometryGlobals");

        private void Start()
        {
            _sphereBuffer = CreateSphereBuffer(_spheresRegistry.Count);
            _geometryGlobals = CreateGeometryBuffer();
        }

        private ComputeBuffer CreateSphereBuffer(int count)
        {
            return new(
                count,
                Marshal.SizeOf<SphereData>(),
                ComputeBufferType.Structured,
                ComputeBufferMode.Dynamic);
        }

        private ConstantBuffer<GeometryGlobals> CreateGeometryBuffer()
        {
            var buffer = new ConstantBuffer<GeometryGlobals>();
            return buffer;
        }

        private void UpdateGeometryBuffer(ConstantBuffer<GeometryGlobals> buffer)
        {
            buffer.UpdateData(GeometryGlobals.From(
                                  _spheresRegistry.Count,
                                  _floorMaterial));
        }
        
        private void UpdateSphereBuffer(ComputeBuffer sphereBuffer)
        {
            var data = _spheresRegistry.Select(x => x.ProvideData()).ToArray();
            sphereBuffer.SetData(data);
        }

        private void UpdateBuffers()
        {
            if (_sphereBuffer?.count != _spheresRegistry.Count)
            {
                _sphereBuffer?.Release();
                _sphereBuffer = CreateSphereBuffer(_spheresRegistry.Count);
            }
            
            UpdateSphereBuffer(_sphereBuffer);
            UpdateGeometryBuffer(_geometryGlobals);
        }

        public void WriteTo(Material material)
        {
            UpdateBuffers();
            material.SetBuffer(_spheres, _sphereBuffer);
            _geometryGlobals.Set(material, _geometryBufferName);
        }

        private void OnDestroy()
        {
            _sphereBuffer?.Release();
        }
        
        private readonly struct GeometryGlobals
        {
            public readonly int SphereCount;
            public readonly float3 FloorAlbedo;
            public readonly float FloorRoughness;
            public readonly float3 FloorEmission;
            public readonly float3 FloorIndexOfRefraction;
            
            private GeometryGlobals(
                int sphereCount,
                float3 floorAlbedo,
                float floorRoughness,
                float3 floorEmission,
                float3 floorIndexOfRefraction)
            {
                SphereCount = sphereCount;
                FloorAlbedo = floorAlbedo;
                FloorRoughness = floorRoughness;
                FloorEmission = floorEmission;
                FloorIndexOfRefraction = floorIndexOfRefraction;
            }

            public static GeometryGlobals From(
                int sphereCount,
                MaterialDataDescription floorMaterial)
            {
                return new(
                    sphereCount,
                    floorMaterial.Color.ToFloat().xyz,
                    floorMaterial.Roughness,
                    floorMaterial.Emission.ToFloat().xyz,
                    floorMaterial.IndexOfRefraction);
            }
        }
    }
}