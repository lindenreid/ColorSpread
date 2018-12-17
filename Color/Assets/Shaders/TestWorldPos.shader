Shader "Custom/TestWorldPos" {
	SubShader {
		Pass {
			HLSLPROGRAM
				#include "LWRP/ShaderLibrary/Core.hlsl"

				#pragma vertex vert
				#pragma fragment frag

				struct VertexInput {
					float4 vertex : POSITION;
				};

				struct VertexOutput { 
					float4 posCS : SV_POSITION;
					float3 posWS : TEXCOORD0;
					float3 posVS : TEXCOORD1;
				};

				VertexOutput vert(VertexInput i) {
					VertexOutput o;
					o.posCS = TransformObjectToHClip(i.vertex.xyz);
					o.posWS = TransformObjectToWorld(i.vertex.xyz);
					o.posVS = TransformWorldToView(o.posWS);

					return o;
				}

				float4 frag(VertexOutput i) : SV_Target {
					return float4(i.posWS, 1.0);
				}
			ENDHLSL
		}
	}
	FallBack "Diffuse"
}
