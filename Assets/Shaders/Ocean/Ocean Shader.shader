Shader "Hidden/Ocean Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tiling ("Tiling", Range(1, 1000)) = 1
		_WaterColour ("Water Colour", Color) = (1, 1, 1, 1)
		_WaveFrequency ("Wave Frequency", Range(0, 100)) = 10
		_WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.1
		_WaveOffsetX ("Wave Off X", Float) = 0
		_WaveOffsetY ("Wave Off Y", Float) = 0
	}
	SubShader
	{

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
			
			sampler2D _MainTex;
			fixed4 _WaterColour;
			float _Tiling;
			float _WaveFrequency;
			float _WaveAmplitude;
			float _WaveOffsetX;
			float _WaveOffsetY;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv * _Tiling;

				uv.x += sin(uv.x * _WaveFrequency + _WaveOffsetX) * _WaveAmplitude;
				uv.y += sin(uv.y * _WaveFrequency + _WaveOffsetY) * _WaveAmplitude;

				fixed4 sand = tex2D(_MainTex, uv);				
				return sand * _WaterColour;
			}
			ENDCG
		}
	}
}
