Shader "custom/MatFX"
{
	Properties
	{
		_mat("Mat Color", Color) = (1,1,1,1)
	}

	SubShader
	{


		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		
		Pass 
		{ 
			Lighting On
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
 
			struct appdata
			{
				half4 vertex	:	POSITION;
				half4 normal	:	NORMAL;
			};
             
			struct v2f
			{
				float4 pos				: SV_POSITION;
                float  uv				: TEXCOORD0;
				float  normal			: TEXCOORD1;
			};


			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.y;

				float3 normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz;

				o.normal = max(0, dot(normal, _WorldSpaceLightPos0));

				return o;
			}

			float4 _mat;

			half3 frag (v2f i) : COLOR
			{
				return _mat * max(i.normal, UNITY_LIGHTMODEL_AMBIENT);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
