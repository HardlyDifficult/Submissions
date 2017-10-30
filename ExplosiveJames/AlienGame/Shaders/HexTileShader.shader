Shader "Custom/HexTileShader" {
	Properties {
		_Alpha("Overlay Alpha", Range(0,1)) = 0.15
		_Overlay("Overlay", 2D) = "white" {}

		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Normal("Normal map", 2D) = "bump" {}

		_Scale("Texture Scale", Range(0.1,5)) = 1
		[MaterialToggle] _WorldUV("World UVs", Float) = 1
		[MaterialToggle] _FlipUV("Flip UVs", Float) = 0
		[MaterialEnum(Zero,0,Sixty,60,OneTwenty,120)] _Rotation("Texture Rotation", Int) = 0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard
			#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Overlay;
		sampler2D _Normal;

		half _Alpha;
		half _WorldUV;
		half _FlipUV;
		half _Scale;
		float _Rotation;
		half _Glossiness;
		half _Metallic;

		fixed4 _Color;

		struct Input {
			float3 worldPos;
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 UV = IN.uv_MainTex;
			// Getting global UVs if true
			if (_WorldUV == 1)
				UV = IN.worldPos.xz;

			// Rotating global UVs
			if (_Rotation != 0) {

				// Almost perfect, as we rotate the texture moves out of sync, this tries to adress it
				float UVoffset = -0.04 * (_Rotation / 120);
				UV += float2(UVoffset, UVoffset);

				// Magic number
				_Rotation *= 0.0174;
				float sinX = sin(_Rotation);
				float cosX = cos(_Rotation);
				float sinY = sin(_Rotation);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
				UV = mul(UV, rotationMatrix);
			}

			// Magic numbers to align texture to the grid world
			if (_WorldUV == 1) {
				UV.x *= 1.15470053838;
				UV.y *= 1.15470053838;
				UV += 1.5;
			}
			if (_FlipUV == 1) {
				UV.y = -UV.y;
			}

			UV /= _Scale;
			
			fixed4 c = tex2D (_MainTex, UV) * _Color;
			fixed4 c2 = tex2D(_Overlay, IN.uv_MainTex);

			o.Albedo = c.rgb + (c2.rgb * _Alpha);
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_Normal, UV));

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
