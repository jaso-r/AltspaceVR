// Shader for highlighting manipulatable objects with an outline.
// The original shader used by the "candy" objects I placed around my scene
// used "Bumped Specular", so I had to mimic that with extra "outline" stuff as well.
// I can't take credit for all the outline code. I copied most of the logic for it
// from a shader I found online.
// Also, I have taken no time to make SubShaders that run on other platforms.
// If you wanted to use this, say, on consoles, you'd probably need to put a little
// more work into it.
//
// Author: Jason Rickwald

Shader "Custom/Outline" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_OutlineColor ("Outline Color", Color) = (0,1,1,1)
		_OutlineWidth ("Outline width", Range (0.0, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	
	SubShader {
		Tags { "Queue" = "Transparent" }
 
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			ZTest Always
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
 
			CGPROGRAM

#include "UnityCG.cginc"

#pragma vertex vertOutline
#pragma fragment fragOutline

struct outlineData {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};
 
struct outlineOut {
	float4 position : POSITION;
	float4 color : COLOR;
};

uniform float _OutlineWidth;
uniform float4 _OutlineColor;

outlineOut vertOutline(outlineData v) {
	outlineOut oVert;
	oVert.position = mul(UNITY_MATRIX_MVP, v.vertex);
 
	float3 n = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	float2 o = TransformViewToProjection(n.xy);
 
	oVert.position.xy += o * oVert.position.z * _OutlineWidth;
	oVert.color = _OutlineColor;
	return oVert;
}

half4 fragOutline(outlineOut i) : COLOR {
	return i.color;
}

			ENDCG
		}
		
		CGPROGRAM

#pragma surface bumpSurf BlinnPhong

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
};
 
sampler2D _MainTex;
sampler2D _BumpMap;
uniform fixed4 _Color;
uniform half _Shininess;

void bumpSurf(Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
			
		ENDCG
		
	}
	FallBack "Bumped Specular"
}
