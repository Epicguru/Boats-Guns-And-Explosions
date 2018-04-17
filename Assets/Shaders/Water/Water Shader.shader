Shader "Hidden/Water Shader"
{
	Properties
	{
		_WaterA ("Water A", 2D) = "white" {}
		_WaterB ("Water B", 2D) = "white" {}
		_Lerp ("Lerp", Range(0, 1)) = 0
		_Colour ("Colour", Color) = (1, 1, 1, 1)
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
			
			sampler2D _WaterA;
			sampler2D _WaterB;
			float _Lerp;
			fixed4 _Colour;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 a = tex2D(_WaterA, i.uv);
				fixed4 b = tex2D(_WaterB, i.uv);

				fixed4 final = lerp(a, b, _Lerp) * _Colour;

				return final;
			}
			ENDCG
		}
	}
}
