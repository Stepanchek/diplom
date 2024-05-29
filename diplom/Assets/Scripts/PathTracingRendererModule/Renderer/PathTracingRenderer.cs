using System;
using PathTracingRendererModule;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public interface IPathTracingRenderer
{
    void UpdateImageSize(int width, int height);
    IPathTracingSample SampleImage(
        IGeometryBus geometryBus, 
        IEnvironmentBus environmentBus, 
        ICameraBus cameraBus);
}

public sealed class PathTracingRenderer : IPathTracingRenderer, IDisposable
{
    private readonly Material _pathTracingMaterial;
    
    private RenderTexture _sampleSource;
    private PathTracingSample _sample;

    private PathTracingRenderer(int imageWidth, int imageHeight, Shader pathTracingShader)
    {
        _sampleSource = CreateRenderTexture(imageWidth, imageHeight);
        _pathTracingMaterial = new Material(pathTracingShader);
    }

    public static PathTracingRenderer From(int imageWidth, int imageHeight, Shader pathTracingShader)
    {
        return new PathTracingRenderer(imageWidth, imageHeight, pathTracingShader);
    }

    private RenderTexture CreateRenderTexture(int width, int height)
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

    public void UpdateImageSize(int width, int height)
    {
        if (_sampleSource != null)
            _sampleSource.Release();
        
        _sampleSource = CreateRenderTexture(width, height);
    }

    public IPathTracingSample SampleImage(IGeometryBus geometryBus, IEnvironmentBus environmentBus, ICameraBus cameraBus)
    {
        _sample?.Invalidate();
        cameraBus.WriteTo(_pathTracingMaterial);
        environmentBus.WriteTo(_pathTracingMaterial);
        geometryBus.WriteTo(_pathTracingMaterial);

        Graphics.Blit(null, _sampleSource, _pathTracingMaterial);
        _sample = PathTracingSample.From(_sampleSource);
        return _sample;
    }
    
    public void Dispose()
    {
        _sampleSource.Release();
        _sample.Invalidate();
    }
}
