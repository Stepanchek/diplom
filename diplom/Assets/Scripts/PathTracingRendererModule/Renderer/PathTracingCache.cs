using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace PathTracingRendererModule
{
    public interface IPathTracingCache
    {
        void CacheSample(IPathTracingSample sample);
        void OutputCache(RenderTexture destination);
        void ClearCache();
        void UpdateTextureDimensions(int width, int height);
    }
    
    public sealed class PathTracingCache : IPathTracingCache, IDisposable
    {
        private readonly Material _cacheFunctionMaterial;
        
        private RenderTexture _cache;
        private int _cacheLength;

        private static readonly int _newSample = Shader.PropertyToID("NewSample");
        private static readonly int _sampleCache = Shader.PropertyToID("SampleCache");
        private static readonly int _length = Shader.PropertyToID("CacheLength");

        private PathTracingCache(Shader pathTraceCacheFunction, int textureWidth, int textureHeight)
        {
            _cacheFunctionMaterial = new Material(pathTraceCacheFunction);
            _cache = new RenderTexture(
                textureWidth,
                textureHeight,
                GraphicsFormat.R32G32B32A32_SFloat,
                GraphicsFormat.None)
            {
                enableRandomWrite = true
            };
        }

        public static PathTracingCache From(int textureWidth, int textureHeight, Shader cacheFunction)
        {
            return new PathTracingCache(cacheFunction, textureWidth, textureHeight);
        }
        
        public void CacheSample(IPathTracingSample sample)
        {
            sample.WriteTo(_cacheFunctionMaterial, _newSample);
            _cacheFunctionMaterial.SetTexture(_sampleCache, _cache);
            _cacheFunctionMaterial.SetInteger(_length, _cacheLength);
            
            var tempRt = RenderTexture.GetTemporary(_cache.descriptor);
            
            Graphics.Blit(null, tempRt, _cacheFunctionMaterial);
            Graphics.Blit(tempRt, _cache);
            RenderTexture.ReleaseTemporary(tempRt);
            
            _cacheLength++;
        }
        
        public void OutputCache(RenderTexture destination)
        {
            Graphics.Blit(_cache, destination);
        }
        
        public void ClearCache()
        {
            _cacheLength = 0;
        }
        
        public void UpdateTextureDimensions(int width, int height)
        {
            ClearCache();
            
            if (_cache != null)
                _cache.Release();

            _cache = CreateTexture(width, height);
        }

        private RenderTexture CreateTexture(int width, int height)
        {
            var rt = new RenderTexture(
                width,
                height,
                GraphicsFormat.R32G32B32A32_SFloat,
                GraphicsFormat.None)
            {
                enableRandomWrite = true
            };
            rt.Create();
            return rt;
        }
        
        public void Dispose()
        {
            _cache.Release();
        }
    }
}