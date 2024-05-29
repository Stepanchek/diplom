using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace RayTracing
{
    public class CacheFunctionDebugger : MonoBehaviour
    {
        [SerializeField] private Shader _cacheShader;

        private Material _material;
        private RenderTexture _cacheSource;
        private int _sampleCount;
        
        private static readonly int _sampleCache = Shader.PropertyToID("SampleCache");
        private static readonly int _cacheLength = Shader.PropertyToID("CacheLength");

        private void Start()
        {
            _material = new Material(_cacheShader);
            _cacheSource = new RenderTexture(
                Screen.width,
                Screen.height,
                GraphicsFormat.R32G32B32A32_SFloat,
                GraphicsFormat.None)
            {
                enableRandomWrite = true
            };
            _cacheSource.Create();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _material.SetTexture(_sampleCache, _cacheSource);
            _material.SetInteger(_cacheLength, _sampleCount);
            _material.SetTexture("NewSample", source);
            
            var temp = RenderTexture.GetTemporary(_cacheSource.descriptor);

            Graphics.Blit(null, temp, _material);
            Graphics.Blit(temp, _cacheSource);
            Graphics.Blit(_cacheSource, destination);
            RenderTexture.ReleaseTemporary(temp);
            _sampleCount++;
        }

        private void OnDestroy()
        {
            _cacheSource.Release();
        }
    }
}