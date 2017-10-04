Shader "Custom/FresnelShader" {
	Properties{
		_MainCol("Main Colour", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

		_Shininess("Fresnel", Range(0.01, 3)) = 1
		_MyColor("Fresnel Color", Color) = (1,1,1,1)

		_Bump("Normal Map", 2D) = "Normal Map" {}

	}
		SubShader{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Bump;
		float _Shininess;
		fixed4 _MyColor;
		fixed4 _MainCol;

	struct Input {
		float4 vertexColor : COLOR;
		float2 uv_MainTex;
		float2 uv_Bump;
		float3 viewDir;
	};
	
	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		c *= IN.vertexColor;
		o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
		half factor = dot(normalize(IN.viewDir),o.Normal);
		float fresVal = _Shininess - factor*_Shininess;
		o.Albedo = ((c.rgb * _MainCol) * (1 - fresVal)) + ((_MyColor *= IN.vertexColor) * fresVal);
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}