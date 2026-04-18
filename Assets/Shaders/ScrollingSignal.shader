Shader "Unlit/ScrollingSignal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color1 ("Color 1", Color) = (1,1,1,1)
        [HDR]_Color2 ("Color 2", Color) = (1,1,1,1)
        _Speed ("Scroll Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
//        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _Speed;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 scroll = -float2(_Time.y * _Speed, 0);
                fixed4 col = tex2D(_MainTex, frac(i.uv + scroll));
                
                //clip(col.r - 0.25);
                clip(col.r+i.uv.y - 0.25);
                clip(col.r+1-i.uv.y-0.25);
                
                //return i.uv.xyxy;


                return lerp(_Color1, _Color2, col.r);
            }
            ENDCG
        }
    }
}
