Shader "Custom/Cloth" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Bump("Normal Map", 2D) = "Normal Map" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _Bump;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert(inout appdata_full v) {
			// Converting object space to world space
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
			float force = clamp(v.vertex.x * 60, 0.0, 1.0);

			v.vertex.x -= sin(_Time.w + (v.vertex.x * 400)) / 2000 * force;
			v.vertex.y += sin(-_Time.w + (v.vertex.x * 200)) / 200 * force;
			v.vertex.z -= sin(_Time.w + (v.vertex.x * 400)) / 2000 * force;
			v.vertex.z -= v.vertex.x / 10;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = (c.rgb * _Color);
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
