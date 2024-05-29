using Unity.Mathematics;
using UnityEngine;

namespace PathTracingRendererModule
{
    public sealed class Application : MonoBehaviour
    {
        [SerializeField] private Shader _pathTracingShader;
        [SerializeField] private Shader _cacheFunctionShader;
        [SerializeField] private CameraBus _cameraBus;
        [SerializeField] private EnvironmentBus _environmentBus;
        [SerializeField] private GeometryBus _geometryBus;

        private PathTracingRenderer _renderer;
        private PathTracingCache _cache;

        private int2 Resolution => new(Screen.width, Screen.height);
        private int2 _lastFrameResolution;
        
        private void Start()
        {
            _renderer = PathTracingRenderer.From(Screen.width, Screen.height, _pathTracingShader);
            _cache = PathTracingCache.From(Screen.width, Screen.height, _cacheFunctionShader);
            _lastFrameResolution = Resolution;
        }

        private void Update()
        {
            if (_lastFrameResolution.x != Resolution.x || _lastFrameResolution.y != Resolution.y)
            {
                _renderer.UpdateImageSize(Resolution.x, Resolution.y);
                _cache.UpdateTextureDimensions(Resolution.x, Resolution.y);
            }
            
            if(Input.GetKeyDown(KeyCode.R))
                _cache.ClearCache();

            _lastFrameResolution = Resolution;
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var sample = _renderer.SampleImage(_geometryBus, _environmentBus, _cameraBus);
            _cache.CacheSample(sample);
            _cache.OutputCache(destination);
        }
    }
}