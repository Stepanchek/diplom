using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace RayTracing
{
    [ExecuteAlways]
    public sealed class HemisphereSampling : MonoBehaviour
    {
        [SerializeField][Range(0, 1)] private float _width;
        [SerializeField] private Transform _light;
        
        private (float theta, float phi) RandomUniform(float2 random)
        {
            return (math.acos(random.x), random.y * math.PI * 2f);
        }

        private (float theta, float phi) RandomCosine(float2 random)
        {
            return (math.acos(math.sqrt(random.x)), random.y * math.PI * 2f);
        }

        private float3 SphericalToCartesian(float theta, float phi)
        {
            return new(
                math.cos(phi) * math.sin(theta),
                math.cos(theta),
                math.sin(phi) * math.sin(theta));
        }

        private void OnDrawGizmos()
        {
            if (_light == null)
                return;
            
            var random = new Random(5436);
            
            for (var i = 0; i < 2048; i++)
            {
                var randomFloat2 = random.NextFloat2();
                var microfacetOrientation = SampleGgxMicrofacet(randomFloat2, _width);
                //var uniform = RandomUniform(randomFloat2);
                //var cosine = RandomCosine(randomFloat2);
                //var theta = math.lerp(uniform.theta, cosine.theta, _interpolationParameter);
                //var phi = math.lerp(uniform.phi, cosine.phi, _interpolationParameter);
                var microfacetNormal = SphericalToCartesian(microfacetOrientation.theta, microfacetOrientation.phi);
                var light = math.normalize(_light.position - transform.position);
                var view = SurfaceReflect(light, microfacetNormal);

                var nDotV = math.dot(transform.up, view);
                
                if(nDotV < 0)
                    view -= (float3)2 * nDotV * transform.up;    
                
                
                Gizmos.color = new(1f, 0.62f, 0.31f);
                Gizmos.DrawSphere(transform.position + (Vector3)view, .01f);
                Gizmos.color = new(1f, 0.26f, 0.09f);
                Gizmos.DrawSphere(transform.position + (Vector3)microfacetNormal, .01f);
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)light);
                Gizmos.DrawLine(transform.position, SurfaceReflect(transform.position + (Vector3)light, transform.up));
            }
        }

        private float3 SurfaceReflect(float3 vector, float3 surfaceNormal)
        {
            return vector - 2 * (vector - math.dot(surfaceNormal, vector) * surfaceNormal);
        }

        private (float theta, float phi) SampleGgxMicrofacet(float2 seed, float width)
        {
            var theta = math.atan(width * math.sqrt(seed.x) / math.sqrt(1 - seed.x));
            var phi = math.PI * 2 * seed.y;

            return (theta, phi);
        }
    }
}