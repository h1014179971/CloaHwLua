Shader "BQ/Particles/Additive" 
{
	Properties 
	{
		_MainTex ("Base", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (0.5, 0.5, 0.5, 0.5)
		_Power ("Power", Range(1.0, 10.0)) = 2.0
		[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _TintColor;
		fixed _Power;
						
		struct v2f 
		{
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 color : COLOR;
		};
		
		float4 _MainTex_ST;

		v2f vert(appdata_full v) 
		{
			v2f o;
			
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.color = v.color;
					
			return o;
		}
		
		fixed4 frag( v2f i ) : COLOR 
		{	
			return tex2D (_MainTex, i.uv) * i.color * _TintColor * _Power;
		}
	
	ENDCG
	
	SubShader 
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1"}
		LOD 100

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha One
		
		Pass 
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 n3ds 
			
			ENDCG
		}
				
	} 
	FallBack Off
}
