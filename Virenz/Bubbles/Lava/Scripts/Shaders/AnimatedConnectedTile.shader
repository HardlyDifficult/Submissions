// ConnectedTile: UV is selected by world position, not local position
// Animation: cycle through a set of frames in the texture
Shader "HD/AnimatedConnectedTile"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Normal("Normal map", 2D) = "bump" {}
		_TimePerFrame("Time per frame", Float) = 1
		_SizeX("Size X", Int) = 8
		_SizeY("Size Y", Int) = 8
		_Color("Color", Color) = (1,1,1,1)
		_XTiling("X Tiling", Float) = 1
		_YTiling("Y Tiling", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Normal;
			float4 _MainTex_ST;
			float _TimePerFrame;
			int _SizeX;
			int _SizeY;
			float4 _Color;
			float _XTiling;
			float _YTiling;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.uv = mul(unity_ObjectToWorld, v.vertex).xz;
				o.uv.x *= _XTiling;
				o.uv.y *= _YTiling;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				int frames = _SizeX * _SizeY;
					
				i.uv += float2(1,1); // Avoid mod negative numbers
				float2 tileSize = float2(1.0 / _SizeX, 1.0 / _SizeY);
				i.uv *= tileSize;
				i.uv.x = fmod(i.uv.x, tileSize.x);
				i.uv.y = fmod(i.uv.y, tileSize.y);

				uint currentFrame = (uint)(_Time.x / _TimePerFrame);
				currentFrame = currentFrame % frames;

				int frameIndex = currentFrame / _SizeX;
				i.uv += float2((float)currentFrame / _SizeX - frameIndex, -frameIndex / (float)_SizeY);       

                half3  Albedo = tex2D(_MainTex, i.uv.xy ).rgb;
                half3 pNormal = UnpackNormal(tex2D(_Normal,  i.uv.xy));
				float3 lightDir = _WorldSpaceLightPos0; // Does not work without "LightMode"="ForwardBase"
				float3 mulled = mul(unity_ObjectToWorld, pNormal);
                half pxlAtten = dot( mulled, lightDir );
                half3 diff = Albedo * pxlAtten;
                return half4( diff, 1 ) * _Color;
			}
			ENDCG
		}
	}
}
