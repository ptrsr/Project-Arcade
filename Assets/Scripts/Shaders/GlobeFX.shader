Shader "custom/globeFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
		 #include "AutoLight.cginc"
		 #include "Lighting.cginc"
	 ENDCG
 
	SubShader
	{
		LOD 200
		Tags { "RenderType"="Opaque" }
		
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

				fixed4	lightDirection	:	TEXCOORD1;
				fixed3	viewDirection	:	TEXCOORD2;
				fixed3	normalWorld		:	TEXCOORD3;
				LIGHTING_COORDS(4,6)
			};
 
			uniform sampler2D _splatmap;

			v2f vert (appdata v)
			{
				v2f o;
                 
				half4 posWorld = mul( unity_ObjectToWorld, v.vertex );
				o.normalWorld = normalize( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz );
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);


				float height = 1 + tex2Dlod(_splatmap, v.uv) / 50;
				o.pos = UnityObjectToClipPos(v.vertex * height);
				o.uv = v.uv;             

				TRANSFER_VERTEX_TO_FRAGMENT(o);
                 
				return o;
			}
             
			half4 frag (v2f i) : COLOR
			{
				fixed NdotL = dot(i.normalWorld, i.lightDirection);
				half atten = LIGHT_ATTENUATION(i);
				fixed3 diffuseReflection = _LightColor0.rgb * atten * float3(1,1,1);
				fixed3 finalColor = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection;
                 
				return float4(finalColor, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
