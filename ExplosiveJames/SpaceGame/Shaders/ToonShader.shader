Shader "Custom/ToonShader" {
	Properties
	{
	_ToonLut("Toon LUT", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags
	{
		"RenderType" = "Opaque"
	}

		Pass
	{
		Tags
	{
		"LightMode" = "ForwardBase"
	}

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal: NORMAL;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD1;
		float3 viewDir : TEXCOORD2;
	};

	sampler2D _ToonLut;
	fixed4 _Color;

	v2f vert(appdata v)
	{
		v2f o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float3 normal = normalize(i.normal);
		float ndotl = dot(normal, _WorldSpaceLightPos0);
		float ndotv = saturate(dot(normal, i.viewDir));

		float3 lut = tex2D(_ToonLut, float2(ndotl, 0));
		float saturation = (lut.x + lut.g + lut.z) / 3;

		fixed4 col = (_Color * (1 - saturation)) + (fixed4(1,1,1,1) * saturation);
		col.a = 1.0;

		return col;
	}
		ENDCG
	}
	}
		Fallback "Diffuse"
}
