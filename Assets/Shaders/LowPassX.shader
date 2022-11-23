Shader "Hidden/LowPassX"
{
    Properties
    {
        _ImTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _ImTex;
            float4 _ImTex_TexelSize;

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 sum = float4(0,0,0,0);

                fixed4 right = tex2D(_ImTex, i.uv + fixed2(_ImTex_TexelSize.x, 0));
                fixed4 center = tex2D(_ImTex, i.uv) * 2;
                fixed4 left = tex2D(_ImTex, i.uv - fixed2(_ImTex_TexelSize.x, 0));

                sum += (left + center + right) * 0.25;

                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //sum.r = 1.0;

                return sum;
            }
            ENDCG
        }

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _ImTex;
            float4 _ImTex_TexelSize;

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 sum = float4(0,0,0,0);

                fixed4 right = tex2D(_ImTex, i.uv + fixed2(0, _ImTex_TexelSize.y));
                fixed4 center = tex2D(_ImTex, i.uv) * 2;
                fixed4 left = tex2D(_ImTex, i.uv - fixed2(0, _ImTex_TexelSize.y));

                sum += (left + center + right) * 0.25;

                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //sum.r = 1.0;

                return sum;
            }
            ENDCG

        }
    }
}
