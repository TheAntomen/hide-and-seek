Shader "Hidden/LowPassX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DecayFactor("Decay Factor", range(0, 1)) = 0.9
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _DecayFactor;
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
			
			// Low-pass filtering in x-axis
			fixed4 frag(v2f i) : SV_TARGET
			{
                // Gaussian weights
                const float w1 = 1.0;
                const float w2 = 4.0;
                const float w3 = 7.0;
                const float w4 = 4.0;
                const float w5 = 1.0;
                const float normFactor = 1.0 / 17.0; // Sum of weights = 17

                // Sampling offsets
                float2 texelX = float2(_MainTex_TexelSize.x, 0);
                float2 texelY = float2(0, _MainTex_TexelSize.y);

                // 5-point Gaussian blur horizontally
                fixed4 left2  = tex2D(_MainTex, i.uv - 2 * texelX) * w1;
                fixed4 left   = tex2D(_MainTex, i.uv - texelX) * w2;
                fixed4 center = tex2D(_MainTex, i.uv) * w3;
                fixed4 right  = tex2D(_MainTex, i.uv + texelX) * w2;
                fixed4 right2 = tex2D(_MainTex, i.uv + 2 * texelX) * w1;

                fixed4 horizontalBlur = (left2 + left + center + right + right2) * normFactor;

                // 5-point Gaussian blur vertically
                fixed4 up2   = tex2D(_MainTex, i.uv + 2 * texelY) * w1;
                fixed4 up    = tex2D(_MainTex, i.uv + texelY) * w2;
                fixed4 down  = tex2D(_MainTex, i.uv - texelY) * w2;
                fixed4 down2 = tex2D(_MainTex, i.uv - 2 * texelY) * w1;

                fixed4 verticalBlur = (up2 + up + center + down + down2) * normFactor;

                // Combined blur
                fixed4 blurredValue = (horizontalBlur + verticalBlur) * 0.5;

                //fixed4 baseGray = fixed4(0.5, 0.5, 0.5, 1.0); // Neutral gray background
				//fixed4 memoryValue = lerp(blurredValue, baseGray, _DecayFactor);

                return blurredValue;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Pass for stepping texel towards 0.5
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
