using UnityEngine;

namespace PathTracingRendererModule
{
    public interface IEnvironmentBus
    {
        void WriteTo(Material material);
    }
    
    public class EnvironmentBus : MonoBehaviour, IEnvironmentBus
    {
        [SerializeField] private Texture _hdrSkybox;
        
        private static readonly int _skybox = Shader.PropertyToID("Skybox");

        public void WriteTo(Material material)
        {
            material.SetTexture(_skybox, _hdrSkybox);
        }
    }
}