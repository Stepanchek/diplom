using UnityEngine;

namespace PathTracingRendererModule
{
    public interface ICameraBus
    {
        void WriteTo(Material material);
    }
    
    public sealed class CameraBus : MonoBehaviour, ICameraBus
    {
        [SerializeField] private Camera _camera;
        
        private static readonly int _cameraAspectRatio = Shader.PropertyToID("Camera_AspectRatio");
        private static readonly int _cameraFov = Shader.PropertyToID("Camera_Fov");
        private static readonly int _cameraUp = Shader.PropertyToID("Camera_Up");
        private static readonly int _cameraForward = Shader.PropertyToID("Camera_Forward");
        private static readonly int _cameraRight = Shader.PropertyToID("Camera_Right");
        private static readonly int _cameraPosition = Shader.PropertyToID("Camera_Position");
        private static readonly int _screenWidth = Shader.PropertyToID("ScreenWidth");
        private static readonly int _screenHeight = Shader.PropertyToID("ScreenHeight");
        
        public void WriteTo(Material material)
        {
            var cameraTransform = _camera.transform;
            material.SetFloat(_cameraAspectRatio, _camera.aspect);
            material.SetFloat(_cameraFov, _camera.fieldOfView * Mathf.Deg2Rad);
            material.SetVector(_cameraUp, cameraTransform.up);
            material.SetVector(_cameraForward, cameraTransform.forward);
            material.SetVector(_cameraRight, cameraTransform.right);
            material.SetVector(_cameraPosition, cameraTransform.position);
            material.SetInteger(_screenHeight, Screen.currentResolution.height);
            material.SetInteger(_screenWidth, Screen.currentResolution.width);
        }
    }
}