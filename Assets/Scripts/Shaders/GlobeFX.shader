// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable

// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "custom/globe_fx"
{
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Pass
		{
			ZWrite On Lighting On
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
#ifdef LIGHTMAP_ON
		// half4 unity_LightmapST;
	sampler2D_half unity_Lightmap;
#endif


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 uv	  : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex		  : SV_POSITION;
				float3 normal		  : NORMAL;
				float2 uv			  : TEXCOORD0;
				float3 view			  : TEXCOORD1;
				float3 lightDirection : TEXCOORD2;

				LIGHTING_COORDS(4, 6)
			};



			uniform sampler2D _splatmap;

			v2f vert (appdata v)
			{
				v2f o;

				o.normal = v.normal;
				o.uv = v.uv;

				float height = 1;// +tex2Dlod(_splatmap, float4(o.uv, 0, 0)).a / 40;
				o.vertex = UnityObjectToClipPos(v.vertex * height);

				half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.view = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return 1 * (DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[1]))) * 2;
			}

			ENDCG
		}
	}
	FallBack "VertexLit"
}