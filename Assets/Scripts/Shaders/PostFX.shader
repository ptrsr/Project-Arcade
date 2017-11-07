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

			// functions
			float3 WorldPosition (float depth, float4 ray);
			float LightValue(float intersectLength, float3 ray, out float3 normal);			
			bool Sphere(float3 ray, out float2 intersections);
			float3 Gradient(float t);

			// uniforms
			uniform float _levelWidth;
			uniform float _borderFade;

			uniform float _qAtmosFallOff;
			uniform float _lAtmosFallOff;

			uniform float _radius;

			uniform float _test1;
			uniform float _test2;

			uniform float4 _gradient[10];


			fixed3 frag (v2f i) : SV_Target
			{
				// scene render
				fixed4 scene = tex2D(_Scene, i.uv);

				// depth sampling
				float linearDepth = Linear01Depth(tex2D(_LastCameraDepthTexture, i.uv));

				float2 inters;

				if (!Sphere(i.ray, inters))
					return 0;

				float lightValue;

				float3 fragPos0;
				float3 fragPos1;


				// calculate lighting values at ray atmosphere intersection
				float p0 = LightValue(inters.x, i.ray, fragPos0);
				float p1 = LightValue(inters.y, i.ray, fragPos1);


				// gray out part of planet which is not part of level
				float3 worldPos = WorldPosition (linearDepth, i.ray);

				float fade = 1 - clamp(_levelWidth - abs(worldPos.z) + (abs(worldPos.z) - _levelWidth) * _borderFade, 0, 1);
				float3 worldColor = lerp(scene, length(scene.xyz) / 3.0 + 0.1f, fade);


				float3 atmosColor = _gradient[0];
				float atmosDistance = length((fragPos0 + fragPos1) / 2) / _radius;

				if (linearDepth == 1) // if the ray doesn't hit the planet
				{
					lightValue = max(p0, p1);
					lightValue *= pow(max(atmosDistance + _test2, 1), _test1);
					
					atmosColor = Gradient(atmosDistance);					

					worldColor = 0;
				}
				else if (inters.x < 0) // if the camera is inside the atmosphere
					lightValue = p1;
				else
					lightValue = p0; // if the camera does hit the planet	

				// fragments world position
				float drawn = any(worldColor) || linearDepth != 1 ? _lAtmosFallOff * pow((1 - abs(dot(normalize(fragPos0), normalize(i.ray)))), _qAtmosFallOff) : 1;

				return lerp(worldColor, atmosColor, min( max(lightValue * drawn, 0), 1));
			}


			float3 WorldPosition (float depth, float4 ray) 
			{
				// calculating fragments world position
				float4 dir = depth * ray;
				float3 pos = _WorldSpaceCameraPos + dir;
				return pos;
			}

			float LightValue(float intersectLength, float3 ray, out float3 fragPos)
			{
				fragPos = intersectLength * ray + _WorldSpaceCameraPos;
				return max(dot(normalize(fragPos), _WorldSpaceLightPos0), 0);
			}

			uniform float _colorKeys;



			bool Sphere(float3 ray, out float2 intersectLengths)
			{
				float2 t;
				// analytic solution
				float3 L = _WorldSpaceCameraPos; 
				float a = dot(ray, ray); 
				float b = 2 * dot(ray, L); 
				float c = dot(L, L) - pow(_radius, 2); 

				float discr = b * b - 4 * a * c; 
				
				if (discr < 0) // ray doesn't intersect sphere 
					return false;
				else if (discr == 0)
				{
					t.x = (-0.5 * b) / a;
					t.y = t.x;
				}
				else
				{
					float q = (b > 0) ?  -0.5 * (b + sqrt(discr)) : -0.5 * (b - sqrt(discr)); 
					t = float2(q / a, c / q); 
				}

				if (t.x > t.y)
					t = t.yx;

				intersectLengths = t;
				return true;



			}

			float3 Gradient(float t)
			{
			 	for (int i = 0; i < _colorKeys; i++)
				{
					if (_gradient[i].a < t)
						continue;

					if (i == 0)
						return _gradient[0].rgb;

					return lerp(_gradient[i - 1].rgb, _gradient[i].rgb,  (1.0 / (_gradient[i].a - _gradient[i-1].a)) * (t - _gradient[i - 1].a));
					break;
				}
				return _gradient[_colorKeys - 1].rgb;
			}

			ENDCG
		}
	}
}