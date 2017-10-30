Shader "Custom/LeavesShader"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

		SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 300

		CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow

		sampler2D _MainTex;
	fixed4 _Color;

	struct Input
	{
		float2 uv_MainTex;
	};

	void vert(inout appdata_full v) {
		// Random offsets
		float timeX = _Time.y * v.vertex.x * 30;
		float timeY = _Time.y * v.vertex.y * 30;

		// Moving verts
		v.vertex.x += sin(timeX * 1.5) / 1200;
		v.vertex.z += sin(timeY) / 1200;
	}


	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 MAIN = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = MAIN.rgb;
		o.Alpha = MAIN.a;
	}
	ENDCG
	}

		FallBack "Transparent/Cutout/Diffuse"
}

