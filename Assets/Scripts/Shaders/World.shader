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
 
			uniform half4        _Color;
 
			struct appdata
			{
				half4	vertex	:	POSITION;
				float2	uv		:	TEXCOORD0;

			};
             
			struct v2g
			{
				half4  pos				:	POSITION;
				float2 uv				:	TEXCOORD0;

				float4 color		    :   TEXCOORD1;
			};
 
			struct g2f
			{
				float4 pos				: SV_POSITION;
                float2 uv				: TEXCOORD0;

                float4 color			: TEXCOORD1;
				float3 normalWorld		: TEXCOORD2;
				float3 viewDirection	: TEXCOORD3;

				SHADOW_COORDS(4)
			};

			uniform float _waterLevel;
			
			uniform float4 _grass;
			uniform float4 _water;


			uniform sampler2D _waveMap;

			uniform float _waveHeight;
			uniform float _waveMulti;

			uniform float _timer;

			v2g vert (appdata v)
			{
				v2g o;

				float4 vertex = v.vertex;
				float4 color = _grass;

				if (length(vertex) < _waterLevel)
				{
					float wave1 = (length(tex2Dlod(_waveMap, float4(v.uv / _waveMulti + float2(-1, 1) * _timer, 0, 0)).rgb) / 3);
					float wave2 = (length(tex2Dlod(_waveMap, float4(v.uv / _waveMulti + float2(1, 1) * _timer, 0, 0)).rgb) / 3);

					float waveHeight = wave1 * wave2 * _waveHeight;

					vertex = normalize(vertex) * (_waterLevel + waveHeight);
					color = _water;
				}
		
				o.color = color;
				o.pos = vertex;
				o.uv = v.uv;             

                 
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> tristream)
			{
				g2f o;
				
                float3 edgeA = IN[1].pos - IN[0].pos;
                float3 edgeB = IN[2].pos - IN[0].pos;
                float3 normal = normalize(cross(edgeA, edgeB));

				for (int i = 0; i < 3; i++)
				{
					o.pos = UnityObjectToClipPos(IN[i].pos);
					o.uv = IN[i].uv;
					o.color = IN[i].color;
					o.normalWorld = normalize(mul(half4(normal, 0.0), unity_WorldToObject).xyz);

					half4 worldPos = mul( unity_ObjectToWorld, IN[i].pos );
					o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

					TRANSFER_SHADOW(o);

					tristream.Append(o);
				}
				tristream.RestartStrip();

			}

			uniform float _ambient;
			uniform float _specular;

			half3 frag (g2f i) : COLOR
			{
				float atten = SHADOW_ATTENUATION(i);

				float diffuse  = max(_ambient, atten * dot(i.normalWorld, _WorldSpaceLightPos0));
				float specular = pow(max(0, dot(reflect(normalize(i.viewDirection), i.normalWorld), -_WorldSpaceLightPos0)) * i.color.a * atten, _specular);
				fixed3 finalColor = _LightColor0.rgb * diffuse * i.color.rgb + specular;

				return finalColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
