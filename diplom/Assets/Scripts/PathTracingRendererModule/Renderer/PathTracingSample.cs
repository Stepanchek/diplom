using System;
using UnityEngine;

namespace PathTracingRendererModule
{
    public interface IPathTracingSample
    {
        void WriteTo(Material material, int propertyId);
        void WriteTo(RenderTexture destination);
    }
    
    public sealed class PathTracingSample : IPathTracingSample
    {
        private readonly RenderTexture _sample;
        private bool _isValid = true;
        
        private PathTracingSample(RenderTexture sample)
        {
            _sample = sample;
        }

        public static PathTracingSample From(RenderTexture sample)
        {
            return new PathTracingSample(sample);
        }
        
        public void WriteTo(Material material, int propertyId)
        {
            if (!_isValid)
                throw new Exception("The sample is invalid");

            material.SetTexture(propertyId, _sample);
        }
        
        public void WriteTo(RenderTexture destination)
        {
            if (!_isValid)
                throw new Exception("The sample is invalid");

            Graphics.Blit(_sample, destination);
        }

        public void Invalidate()
        {
            _isValid = false;
        }
    }
}