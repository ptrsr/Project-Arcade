Shader "custom/globeFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
		 #include "AutoLight.cginc"
		 #include "Lighting.cginc"
	 ENDCG
 
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		
		Pass 
		{ 
			Lighting On
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			struct appdata
			{
				half4	vertex	:	POSITION;
				float2	uv		:	TEXCOORD0;
				uint    id		:	SV_VertexID;

			};
             
			struct v2g
			{
				half4  vertex			:	POSITION;
				float4 splat			:	TEXCOORD0;
				float4 paint			:	TEXCOORD1;
			};
 
			struct g2f
			{
				float4 pos				: SV_POSITION;
                float4 splat			: TEXCOORD1;
				float4 paint			: TEXCOORD2;
				float4 normalWorld		: TEXCOORD3;
				float3 viewDirection	: TEXCOORD4;

				SHADOW_COORDS(5)
			};

			uniform float _waterLevel;
			



			uniform sampler2D _waveMap;

			uniform float _waveHeight;
			uniform float _waveMulti;

			uniform float _timer;

			uniform sampler2D _paintMap;
			uniform float2 _paintMapSize;

			v2g vert (appdata v)
			{
				v2g o;

				if (length(v.vertex) < _waterLevel)
				{
					float wave1 = (length(tex2Dlod(_waveMap, float4(v.uv / _waveMulti + float2(-1, 1) * _timer, 0, 0)).rgb) / 3);
					float wave2 = (length(tex2Dlod(_waveMap, float4(v.uv / _waveMulti + float2(1, 1) * _timer, 0, 0)).rgb) / 3);

					float waveHeight = wave1 * wave2 * _waveHeight;

					o.vertex = normalize(v.vertex) * (_waterLevel + waveHeight);

					o.splat = float4(0,0,1,0);
					o.paint = float4(0,0,0,0);

					return o;
				}
				
				o.vertex = v.vertex;

				float2 delta = float2(0.5 / _paintMapSize.x, 0.5 / _paintMapSize.y);
				o.paint = tex2Dlod(_paintMap, float4((v.id % uint(_paintMapSize.x)) / _paintMapSize.x + delta.x, (v.id / uint(_paintMapSize.x)) / _paintMapSize.y + delta.y, 0, 0));
                
				o.splat = float4(0, 1, 0, o.paint.a);
				
 
				return o;
			}


			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> tristream)
			{
				g2f o;
				
				// calculating triangle normal;
                float3 edgeA = IN[1].vertex - IN[0].vertex;
                float3 edgeB = IN[2].vertex - IN[0].vertex;
                float3 normal = normalize(cross(edgeA, edgeB));

				float4 splat = float4(0, 0, 0, -1);

				float4 test = IN[0].splat + IN[1].splat + IN[2].splat;

				if (test.g > 0 && test.b > 0)
					splat = float4(1, -1, 0, -1);

				if (IN[0].splat.w > 0 && IN[1].splat.w > 0 && IN[2].splat.w > 0)
					splat = float4(-1, -1, -1, 1);

				// drawing triangle
				for (int i = 0; i < 3; i++)
				{
					o.pos = UnityObjectToClipPos(IN[i].vertex);

					o.splat = clamp(IN[i].splat + splat, float4(0,0,0,0), float4(1,1,1,1));

					o.paint = IN[i].paint;

					o.normalWorld = float4(normalize(mul(half4(normal, 0.0), unity_WorldToObject).xyz), length(IN[i].vertex));

					half4 worldPos = mul( unity_ObjectToWorld, IN[i].vertex );
					o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

					TRANSFER_SHADOW(o);

					tristream.Append(o);
				}
				tristream.RestartStrip();
			}


			uniform float _ambient;
			uniform float _specular;
			uniform float _test;

			uniform float4 _grass;
			uniform float4 _water;
			uniform float4 _sand;

			half3 frag (g2f i) : COLOR
			{
				float4 splat = i.splat;
				//splat.r = clamp(splat.r - splat.b, 0, 1);
				
				if (splat.r > 0)
				{
					if (i.normalWorld.w > _waterLevel + _test)
						splat = clamp(float4(1 - (splat.r - splat.b), (splat.r - splat.b),0,1) , float4(0,0,0,0), float4(1,1,1,1));
					else
						splat = float4(0,0,1,0);
				}

				float4 color = splat.r * _sand + splat.g * _grass + splat.b * _water + splat.a * i.paint;

				float atten = SHADOW_ATTENUATION(i);

				float diffuse  = max(_ambient, atten * dot(i.normalWorld, _WorldSpaceLightPos0));
				float specular = pow(max(0, dot(reflect(normalize(i.viewDirection), i.normalWorld), -_WorldSpaceLightPos0)) * color.a * atten, _specular);
				fixed3 finalColor = _LightColor0.rgb * diffuse * color + specular;

				return finalColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
