Shader "custom/post_fx"
{
	Properties
	{
		_Scene ("Scene", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 uv	  : TEXCOORD0;
				float4 ray	  : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv	  : TEXCOORD0;
				float4 ray	  : TEXCOORD1;
			};

			uniform sampler2D _Scene;
			uniform sampler2D _CameraDepthTexture;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				o.ray = v.ray;
				return o;
			}

			//functions
			float3 worldPosition (float depth, float4 ray);

			uniform float _levelWidth;
			uniform float _borderFade;

			fixed4 frag (v2f i) : SV_Target
			{
				// scene render
				fixed4 scene = tex2D(_Scene, i.uv);

				// depth sampling
				float linearDepth;
				float3 viewNormal;

				linearDepth = 1 - tex2D(_CameraDepthTexture, i.uv);

				// fragments world position
				float3 worldPos = worldPosition (linearDepth, i.ray);

				if (linearDepth == 1)
					return worldPos.y;
					//return float4(0,0,1, 1);

				float fade = 1 - clamp(_levelWidth - abs(worldPos.z) + (abs(worldPos.z) - _levelWidth) * _borderFade, 0, 1);
				return lerp(scene, length(scene.xyz) / 3.0 + 0.1f, fade);
			}


			float3 worldPosition (float depth, float4 ray) 
			{
				// calculating fragments world position
				float4 dir = depth * ray;
				float3 pos = _WorldSpaceCameraPos + dir;
				return pos;
			}

			ENDCG
		}
	}
}