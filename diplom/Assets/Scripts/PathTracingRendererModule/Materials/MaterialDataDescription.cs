using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PathTracingRendererModule.Materials
{
    [Serializable]
    public struct MaterialDataDescription
    {
        [ColorUsage(false, true)] 
        public Color Color;
        
        [Range(0, 1)] 
        public float Roughness;
        
        [ColorUsage(false, true)] 
        public Color Emission;

        [FormerlySerializedAs("RefractionIndex")]
        public float IndexOfRefraction;
    }
}