using UnityEngine;

[ExecuteAlways]
public class RayDebugger : MonoBehaviour
{
    private Camera _camera;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnDrawGizmos()
    {
        if (_camera == null)
            return; 
        
        for (var i = 0; i <= 11; i++)
        {
            for (var j = 0; j <= 11; j++)
            {
                var halfFov = _camera.fieldOfView * Mathf.Deg2Rad * .5f;
                var distanceToScreen = Mathf.Cos(halfFov) / Mathf.Sin(halfFov);
                var rayBase = new Vector3(0, 0, distanceToScreen);
                var offset = new Vector3(_camera.aspect * (i / 11.0f - .5f) * 2, (j / 11.0f - .5f) * 2, 0);
                var ray = Vector3.Normalize(rayBase + offset);

                Gizmos.color = new(1f, 0.29f, 0.09f);
                var worldSpaceRay = (Vector3)(_camera.cameraToWorldMatrix *ray);
                worldSpaceRay.z *= -1;
                var position = _camera.transform.position;
                Gizmos.DrawLine( position, position + worldSpaceRay * 100);
            }
        }
    }
}
