Shader "custom/text_fx"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }
 
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
	
    fixed4 _TextureSampleAdd;
 
    struct appdata
    {
        float4 vertex   : POSITION;
        float2 uv		: TEXCOORD0;
    };
 
    struct v2f
    {
        float4 vertex   : SV_POSITION;
        half2  uv		: TEXCOORD0;
    };
 
    v2f vert(appdata i)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(i.vertex);
 
        o.uv = i.uv;
     
        return o;
    }
 
    uniform sampler2D _MainTex;
	uniform float4 _color;
	uniform float _width;
	uniform float _timer;
	uniform float _min;
	uniform float _on;

    fixed4 frag(v2f i) : SV_Target
    {
		float strobe = min(max(sin(i.uv.y * _width + _timer), _min), _on);

		float text = tex2D(_MainTex, i.uv).a;
        return float4(_color.rgb * _color.a * 1.2, text * strobe);
    }
    ENDCG
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
		Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        ENDCG
        }
    }
}
