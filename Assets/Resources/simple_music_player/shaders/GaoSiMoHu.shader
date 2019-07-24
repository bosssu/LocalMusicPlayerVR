// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageEffect_GaussianBlur" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_blurSize("blurSize",Range(0,5)) = 1
		_Color("Color",Color) = (1,1,1,1)

	}

		CGINCLUDE

#include "UnityCG.cginc"  

	sampler2D _MainTex;
	uniform half4 _MainTex_TexelSize;
	uniform float _blurSize;
	fixed4 _Color;

	// weight curves  

	static const half curve[4] = { 0.0205, 0.0855, 0.232, 0.324 };
	static const half4 coordOffs = half4(1.0h, 1.0h, -1.0h, -1.0h);

	struct v2f_withBlurCoordsSGX
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 offs[3] : TEXCOORD1;
	};


	v2f_withBlurCoordsSGX vertBlurHorizontalSGX(appdata_img v)
	{
		v2f_withBlurCoordsSGX o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord.xy;
		half2 netFilterWidth = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _blurSize;
		half4 coords = -netFilterWidth.xyxy * 3.0;

		o.offs[0] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[1] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[2] = v.texcoord.xyxy + coords * coordOffs;

		return o;
	}

	v2f_withBlurCoordsSGX vertBlurVerticalSGX(appdata_img v)
	{
		v2f_withBlurCoordsSGX o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord.xy;
		half2 netFilterWidth = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _blurSize;
		half4 coords = -netFilterWidth.xyxy * 3.0;

		o.offs[0] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[1] = v.texcoord.xyxy + coords * coordOffs;
		coords += netFilterWidth.xyxy;
		o.offs[2] = v.texcoord.xyxy + coords * coordOffs;

		return o;
	}

	half4 fragBlurSGX(v2f_withBlurCoordsSGX i) : SV_Target
	{
		half2 uv = i.uv;

		half4 color = tex2D(_MainTex, i.uv) * curve[3];

		color += (tex2D(_MainTex, i.offs[0].xy) + tex2D(_MainTex, i.offs[0].zw)) * curve[0];
		color += (tex2D(_MainTex, i.offs[1].xy) + tex2D(_MainTex, i.offs[1].zw)) * curve[1];
		color += (tex2D(_MainTex, i.offs[2].xy) + tex2D(_MainTex, i.offs[2].zw)) * curve[2];

		return color * _Color;
	}

		ENDCG

		SubShader {



			Pass{


				CGPROGRAM

				#pragma vertex vertBlurVerticalSGX  
				#pragma fragment fragBlurSGX  

				ENDCG
		}


			Pass{
				CGPROGRAM

				#pragma vertex vertBlurHorizontalSGX  
				#pragma fragment fragBlurSGX  

				ENDCG
		}
	}

	FallBack Off
}