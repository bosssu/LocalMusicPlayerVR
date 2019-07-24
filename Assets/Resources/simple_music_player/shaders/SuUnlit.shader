// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SuUnlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Outline("Outline",float) = 1
		_OutlineColor("OutlineColor",Color) = (0,0,0,1)
	}
	SubShader
	{
		
		LOD 100

		
		Pass{
		NAME "OUTLINE"

		Tags{ "RenderType" = "Opaque" }
		Cull Front

		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

		float _Outline;
	fixed4 _OutlineColor;

	struct a2v {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
	};

	v2f vert(a2v v) {
		v2f o;

		float4 pos = mul(UNITY_MATRIX_MV, v.vertex);
		float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		normal.z = -0.5;
		pos = pos + float4(normalize(normal), 0) * _Outline * 0.04;
		o.pos = mul(UNITY_MATRIX_P, pos);

		return o;
	}

	float4 frag(v2f i) : SV_Target{
		return float4(_OutlineColor.rgb, 1);
	}

		ENDCG
	}

		Pass
		{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
