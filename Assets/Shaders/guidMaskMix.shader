﻿Shader "Guid/MaskMix"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15


		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0


			//-------------------add----------------------
			  _Center("Center", vector) = (0, 0, 0, 0)
			  [Toggle]_IsCircle("IsCircle",Float) = 0
			   _Silder("_Silder", Range(0,1000)) = 1000 // sliders
			   _SliderX("_SliderX", Range(0,1000)) = 1000 // sliders
			   _SliderY("_SliderY", Range(0,1000)) = 1000
			//-------------------add----------------------
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO

				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				//-------------------add----------------------
				bool _IsCircle;
				float _Silder;
				float2 _Center;
				float _SliderX;
				float _SliderY;
				//-------------------add----------------------
				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = IN.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

					OUT.texcoord = IN.texcoord;

					OUT.color = IN.color * _Color;
					return OUT;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif
					//-------------------add----------------------
					if (_IsCircle) {
						color.a *= (distance(IN.worldPosition.xy, _Center.xy) > _Silder);
						color.rgb *= color.a;
					}
					else {
						float2 A = _Center.xy + float2(0, _SliderY) + (float2(-_SliderX, 0));
						float2 B = _Center.xy + float2(0, -_SliderY) + (float2(-_SliderX, 0));
						float2 C = _Center.xy + float2(_SliderX, 0) + float2(0, -_SliderY);
						float2 M = IN.worldPosition.xy;
						float2 BC = C - B;
						float2 AB = B - A;
						float2 BM = M - B;
						float2 AM = M - A;  
						color.a *= !(dot(AB, AM) >= 0 && dot(AB, AM) <= dot(AB, AB) && dot(BC, BM) >= 0 && dot(BC, BM) <= dot(BC, BC));
						color.rgb *= color.a;
					}
					
					//-------------------add----------------------
					return color;

				}
			ENDCG
			}
		}
}