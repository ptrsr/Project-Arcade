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
			uniform sampler2D _LastCameraDepthTexture;

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
			float3 skyBox(float3 worldPos);

			uniform float _levelWidth;
			uniform float _borderFade;

			float _skyMulti;


			fixed4 frag (v2f i) : SV_Target
			{
				// scene render
				fixed4 scene = tex2D(_Scene, i.uv);

				// depth sampling
				float linearDepth;

				linearDepth = Linear01Depth(tex2D(_LastCameraDepthTexture, i.uv));

				// fragments world position
				float3 worldPos = worldPosition (linearDepth, i.ray);

				if (linearDepth == 1)
					return float4(skyBox(worldPosition(linearDepth, i.ray * _skyMulti)), 1);
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

			float4 _gradient[10];
			float _colorKeys;

			float3 skyBox(float3 worldPos)
			{

				float2 pos = normalize(worldPos.xy);

				float rot = (dot(pos, float2(0,-1)) + 1) / 2;

				for (int i = 0; i < _colorKeys; i++)
				{
					if (_gradient[i].a < rot)
						continue;

					if (i == 0)
						return _gradient[0].rgb;

					return lerp(_gradient[i - 1].rgb, _gradient[i].rgb,  (1.0 / (_gradient[i].a - _gradient[i-1].a)) * (rot - _gradient[i - 1].a));
				}
				return _gradient[_colorKeys - 1].rgb;
			}


			ENDCG
		}
	}
}