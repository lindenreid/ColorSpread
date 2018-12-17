#ifndef COLOR_SPREAD
#define COLOR_SPREAD

#include "PostProcessing/Shaders/StdLib.hlsl"
#include "PostProcessing/Shaders/Sampling.hlsl"

// Depth texture
TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
// Camera texture 
TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
// Material properties
float _Blend;
float _GrowthSpeed;
float _MaxSize;
float _StartTime;
float _K;
float _P;
float4 _Center;
// Unity properties (set in ColorSpread.cs)
float4x4 unity_ViewToWorldMatrix;
float4x4 unity_InverseProjectionMatrix;

struct VertexInput {
    float4 vertex : POSITION;
};

struct VertexOutput {
    float4 pos : SV_POSITION;
    float2 screenPos : TEXCOORD0;
};

float3 GetWorldFromViewPosition (VertexOutput i) {
    // get view space position
    // thank you to the built-in pp screenSpaceReflections effect for this code lel
    float z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.screenPos).r;
    float4 result = mul(unity_InverseProjectionMatrix, float4(2.0 * i.screenPos - 1.0, z, 1.0));
    float3 viewPos = result.xyz / result.w;

    // get ws position
    float3 worldPos = mul(unity_ViewToWorldMatrix, float4(viewPos, 1.0));

    return worldPos;
    //return viewPos; // TEST
    //return float3(z, z, z); // TEST
}

// Based off of the smoothstep() function
// But instead of using a Hermite curve,
// use e^x with some parameters
float Blend(float min, float max, float x) {
    x = (x - min)/(max-min);
    x = clamp(x, 0.0, 1.0);
    x = exp(-pow(_K*(1 - x), _P));
    return x;
}

VertexOutput Vertex(VertexInput i) {
    VertexOutput o;
    o.pos = float4(i.vertex.xy, 0.0, 1.0);
    
    // get clip space coordinates for sampling camera tex
    o.screenPos = TransformTriangleVertexToUV(i.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
    o.screenPos = o.screenPos * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

    return o;
}

float4 Frag(VertexOutput i) : SV_Target
{
    float3 worldPos = GetWorldFromViewPosition(i);
    //float4 depthColor = float4(worldPos, 1.0); // TEST

    // regular color 
    float4 fullColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.screenPos);

    // grayscale
    float luminance = dot(fullColor.rgb, float3(0.2126729, 0.7151522, 0.0721750));
    float3 greyscale = lerp(fullColor.rgb, luminance.xxx, _Blend.xxx);

    // calculate radius based on animation starting time & current time
    float timeElapsed = _Time.y - _StartTime;
    float currentRadius = min(timeElapsed * _GrowthSpeed, _MaxSize);

    // check if distance is inside bounds of max radius
    // choose greyscale if outside, full color if inside
    float dist = distance(_Center, worldPos);
    
    //float useColor = step(dist, currentRadius);
    //float useColor = smoothstep(currentRadius, 0, dist);
    //float3 color = useColor*fullColor + (1-useColor)*greyscale; // USE FOR TWO ABOVE

    float blend = Blend(0, currentRadius, dist);
    float3 color = (1-blend)*fullColor + blend*greyscale;
    
    return float4(color, 1.0);
    //return depthColor; // TEST
}

#endif