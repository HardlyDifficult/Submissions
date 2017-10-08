// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/UVScroll" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ScrollSpeedX ("X Scroll Speed", Float) = 0
		_ScrollSpeedY ("Y Scroll Speed", Float) = 0
		_Amount ("Wobble Amount", Range(-1, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _ScrollSpeedX;
		fixed _ScrollSpeedY;
		float _Amount;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void vert (inout appdata_full v){
			fixed varY = _ScrollSpeedY * _Time;
			float2 uv_Tex = v.texcoord.xy + fixed2(v.texcoord.x, varY);
			fixed4 c = tex2Dlod(_MainTex, float4(uv_Tex,0,0));
			v.vertex.y += c.a * _Amount * v.normal;

		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed varX = _ScrollSpeedX * _Time;
			fixed varY = _ScrollSpeedY * _Time;
			fixed2 uv_Tex = IN.uv_MainTex + fixed2(varX, varY);
			fixed4 c = tex2D (_MainTex, uv_Tex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
