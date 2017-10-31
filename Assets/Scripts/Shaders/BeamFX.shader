Shader "custom/BeamFX"
{
	 CGINCLUDE
		 #include "UnityCG.cginc"
	 ENDCG
 
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  "ForceNoShadowCasting" = "True" }
		
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass 
		{ 
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			struct appdata
			{
				half4	vertex	:	POSITION;
			};
             
			struct v2f
			{
				float4 pos				: SV_POSITION;
                float  uv				: TEXCOORD0;
			};


			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = length(o.pos);


				return o;
			}


			uniform float  _time;
			uniform float  _effectMulti;
			uniform float4 _color;
	

			half4 frag (v2f i) : COLOR
			{
				float4 color = float4(_color.xyz * _color.a * 2, clamp(sin((i.uv * _effectMulti) + _time), 0, 1));

				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
