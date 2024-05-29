using Unity.Mathematics;
using UnityEngine;
namespace RayTracing
{
    public class RotationDebugger : MonoBehaviour
    {
        [SerializeField] private float3 _from;
        [SerializeField] private float3 _to;
        [SerializeField][Range(-1, 1)] private float _value;

        private float3 RotateAround(float3 vector, float3 axis, float theta)
        {
            var sinTheta = math.sin(theta);
            var cosTheta = math.cos(theta);

            return vector * cosTheta + math.cross(axis, vector) * sinTheta + axis * (math.dot(axis, vector)) * (1 - cosTheta);
        }

        private float3 RotateHemisphereSample(
            float3 from,
            float3 to,
            float3 hemisphereSample)
        {
            var axis = math.normalize(math.cross(from, to));
            var angle = math.acos(math.dot(from, to));

            return RotateAround(hemisphereSample, axis, angle);
        }

        private void OnDrawGizmos()
        {
            var from = _from;
            var to = math.normalize(_to);
            var position = (float3)transform.position;
            var angle = math.lerp(0, math.PI, _value);

            var rotatedVector = RotateHemisphereSample(Vector3.up, transform.up, from);
            
            Gizmos.DrawLine(position, position + rotatedVector);
        }
    }
}