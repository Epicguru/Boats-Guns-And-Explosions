Shader "Hidden/Warp Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_T ("Change", Range(-10, 10)) = 0
		_Center ("Center", Vector) = (0.5, 0.5, 0, 0)
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
			
			sampler2D _MainTex;
			float _T;
			float2 _Center;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 off = float2(0, 0);
				float t = distance(_Center, i.uv) * _T;
				off += (i.uv - _Center) * t * distance(_Center, i.uv);

				fixed4 col = tex2D(_MainTex, i.uv + off);

				return col;
			}
			ENDCG
		}
	}
}
