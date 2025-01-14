Shader "Pink/HealthBar "
{
    Properties
    {
        _Health("Health",Range(0,1))=0
        _LowColor("LowHealth",Color) = (1,0,0,1) 
        _HighColor("HighHealth",Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            float _Health;
            fixed4 _LowColor;
            fixed4 _HighColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv =v.uv;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if(i.uv.x>_Health){
                    return _HighColor;
                }
                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(_LowColor,col,_Health);
            }

            ENDCG
        }
    }
}
