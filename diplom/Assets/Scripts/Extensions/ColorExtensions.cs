using Unity.Mathematics;
using UnityEngine;
namespace PathTracingRendererModule.Extensions
{
    public static class ColorExtensions
    {
        public static float4 ToFloat(this Color color)
        {
            return new(
                color.r,
                color.g,
                color.b,
                color.a);
        }
    }
}