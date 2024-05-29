#include <Assets/Shaders/PathTracing/PathTracer.hlsl>
#include "GeometryInput.hlsl"

fixed4 Frag (const V2F i) : SV_Target
{
    return fixed4(PathTraceScene(
        i.uv,
        ProvideGeometryStructure(),
        ProvideSkyData(),
        ProvideCameraData(),
        asuint(_Time.x)),
        1);
}
