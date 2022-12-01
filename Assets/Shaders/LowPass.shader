Shader "Hidden/LowPassX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

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
	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			fixed4 frag(v2f i) : SV_TARGET
			{

				fixed4 right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0));
				fixed4 center = tex2D(_MainTex, i.uv) * 10;
				fixed4 left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0));

				fixed4 sum = round(((left + center + right)/12)*100)/100;

				return sum;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_TARGET
			{
				fixed4 right = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y));
				fixed4 center = tex2D(_MainTex, i.uv) * 10;
				fixed4 left = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y));

				fixed4 sum = round(((left + center + right)/12)*100)/100;

				return sum;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_TARGET
			{
				fixed4 center = round(tex2D(_MainTex, i.uv)*100)/100;

				if (center.r > 0.5f)
				{
					center -= fixed4(0.01f, 0.01f, 0.01f, 1.0f);
				}
				else if (center.r < 0.5f)
				{
					center += fixed4(0.01f, 0.01f, 0.01f, 1.0f);
				}

				return center;
				}

			ENDCG
		}
	}
}
