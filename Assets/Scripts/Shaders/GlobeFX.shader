Shader "custom/globeFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
		 #include "AutoLight.cginc"
		 #include "Lighting.cginc"
	 ENDCG
 
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
		
		Pass 
		{ 
			Lighting On
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
 
			uniform half4        _Color;
 
			struct appdata
			{
				half4	vertex	:	POSITION;
				half3	normal	:	NORMAL;
				float4	uv		:	TEXCOORD0;

			};
             
			struct v2f
			{
				half4	pos				:	SV_POSITION;
				float4	uv				:	TEXCOORD0;

				fixed3	viewDirection	:	TEXCOORD1;
				fixed3	normalWorld		:	TEXCOORD2;
				LIGHTING_COORDS(4,6)
			};
 
			v2f vert (appdata v)
			{
				v2f o;
                 
				half4 posWorld = mul( unity_ObjectToWorld, v.vertex );
				o.normalWorld = normalize( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz );
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;             

				TRANSFER_VERTEX_TO_FRAGMENT(o);
                 
				return o;
			}

			half3 frag (v2f i) : COLOR
			{
				fixed NdotL = dot(i.normalWorld, _WorldSpaceLightPos0);
				half atten = LIGHT_ATTENUATION(i);
				fixed3 diffuseReflection = _LightColor0.rgb * atten;
				fixed3 finalColor = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection;
                
				return finalColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
