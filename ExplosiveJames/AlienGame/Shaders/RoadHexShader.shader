Shader "Custom/RoadHexShader"
{
	Properties{
		_Alpha("Overlay Alpha", Range(0,1)) = 0.15
		_Overlay("Overlay", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_Normal("Normal map", 2D) = "bump" {}

		[MaterialEnum(NormalAlignment,0,SwappedAlignment,1)] _Offset("Texture Offset", Int) = 0
		[MaterialEnum(N,0,NW,60,SW,120,S,180,SE,240,NE,300)] _Rotation("Texture Rotation", Int) = 0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _Overlay;
	sampler2D _Normal;

	half _Alpha;
	float _Rotation;
	float _Offset;
	half _Glossiness;
	half _Metallic;

	fixed4 _Color;

	struct Input 
	{
		float3 worldPos;
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) 
	{
		// Getting global UVs
		float2 UV = IN.worldPos.xz;

		// Rotating global UVs
		if (_Rotation != 0) {

			// Almost perfect, as we rotate the texture moves out of sync, this tries to adress it
			float UVoffset = -0.04 * (_Rotation / 120);
			UV += float2(UVoffset, UVoffset);

			// Magic number
			float rotation = _Rotation *= 0.0174;
			float sinX = sin(rotation);
			float cosX = cos(rotation);
			float sinY = sin(rotation);
			float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
			UV = mul(UV, rotationMatrix);
		}

		// Magic numbers
		UV.x *= 1.15470053838;
		UV.y *= 1.15470053838;

		// Applying Scale and alignment
		UV /= float2(3, 3);
		UV += float2(.5 * _Offset,.5 * _Offset);

		fixed4 c = tex2D(_MainTex, UV + 1.83) * _Color;
		fixed4 c2 = tex2D(_Overlay, IN.uv_MainTex);

		o.Albedo = c.rgb + (c2.rgb * _Alpha);
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_Normal, UV + 1.85));

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
