Shader "Custom/Crops" {
	Properties{
		_MainCol("Main Colour", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

	_Shininess("Fresnel", Range(0.01, 3)) = 1
		_MyColor("Fresnel Color", Color) = (1,1,1,1)

		_Bump("Normal Map", 2D) = "Normal Map" {}

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		[MaterialEnum(none,0,Crops,1,Grass,2)] _Behaviour("Behaviour Type", Int) = 0

	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow

		sampler2D _MainTex;
	sampler2D _Bump;
	float _Shininess;
	uint _Behaviour;
	fixed4 _MyColor;
	fixed4 _MainCol;

	struct Input {
		float4 vertexColor : COLOR;
		float2 uv_MainTex;
		float2 uv_Bump;
		float3 viewDir;
	};

	void vert(inout appdata_full v) {
		
		// Converting verts to world position
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

		// Crops
		if (_Behaviour == 1) {
			v.vertex.x += (sin((_Time.y / 1.5) + (worldPos.x)) / 40) * v.vertex.z;
			v.vertex.y += (sin((_Time.y / 2) + (worldPos.y)) / 40) * v.vertex.z;
		}
		// Grass
		else if (_Behaviour == 2) {
			float strength = v.vertex.z * 400;

			v.vertex.x += sin((_Time.y / 2) + (worldPos.x * 20)) / 3000 * strength;
			v.vertex.y += sin((_Time.y / 1.5) + (worldPos.z * 20)) / 3000 * strength;
		}
	}

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