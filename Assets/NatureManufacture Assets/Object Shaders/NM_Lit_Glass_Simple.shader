Shader "NatureManufacture/Lit/Glass Simple"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseColorMap("Base Map(RGB) Alpha(A)", 2D) = "white" {}
		_BaseNormalMap("Base Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 8)) = 1
		_BaseMaskMap("Base Mask Map MT(R) AO(G) SM(A)", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 1
		_BaseAORemapMin("AO Remap Min", Range( 0 , 1)) = 0
		_BaseAORemapMax("AO Remap Max", Range( 0 , 1)) = 1
		_BaseSmoothnessRemapMin("Base Smoothness Remap Min", Range( 0 , 1)) = 0
		_BaseSmoothnessRemapMax("Base Smoothness Remap Max", Range( 0 , 1)) = 1
		[HDR]_ThicknessEmission("Thickness Emission", Color) = (0,0,0,0)
		_Thickness("Thickness", 2D) = "white" {}
		_ThicknessRemapMin("Thickness Remap Min", Float) = 0
		_ThicknessRemapMax("Thickness Remap Max", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			half ASEIsFrontFacing : VFACE;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseNormalMap);
		uniform float4 _BaseNormalMap_ST;
		SamplerState sampler_Linear_Repeat;
		uniform float _NormalScale;
		uniform float4 _BaseColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseColorMap);
		uniform float4 _BaseColorMap_ST;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Thickness);
		uniform float4 _Thickness_ST;
		uniform float _ThicknessRemapMin;
		uniform float _ThicknessRemapMax;
		uniform float4 _ThicknessEmission;
		uniform float _Metallic;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseMaskMap);
		uniform float4 _BaseMaskMap_ST;
		uniform float _BaseSmoothnessRemapMin;
		uniform float _BaseSmoothnessRemapMax;
		uniform float _BaseAORemapMin;
		uniform float _BaseAORemapMax;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseNormalMap = i.uv_texcoord * _BaseNormalMap_ST.xy + _BaseNormalMap_ST.zw;
			float3 tex2DNode307 = UnpackNormal( SAMPLE_TEXTURE2D( _BaseNormalMap, sampler_Linear_Repeat, uv_BaseNormalMap ) );
			float3 switchResult336 = (((i.ASEIsFrontFacing>0)?(tex2DNode307):(( tex2DNode307 * float3( 1,1,-1 ) ))));
			float3 break31_g55 = switchResult336;
			float2 appendResult35_g55 = (float2(break31_g55.x , break31_g55.y));
			float temp_output_38_0_g55 = _NormalScale;
			float lerpResult36_g55 = lerp( 1.0 , break31_g55.z , saturate( temp_output_38_0_g55 ));
			float3 appendResult34_g55 = (float3(( appendResult35_g55 * temp_output_38_0_g55 ) , lerpResult36_g55));
			o.Normal = appendResult34_g55;
			float3 appendResult64 = (float3(_BaseColor.r , _BaseColor.g , _BaseColor.b));
			float2 uv_BaseColorMap = i.uv_texcoord * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
			float4 tex2DNode306 = SAMPLE_TEXTURE2D( _BaseColorMap, sampler_Linear_Repeat, uv_BaseColorMap );
			o.Albedo = ( float4( appendResult64 , 0.0 ) * tex2DNode306 ).rgb;
			float2 uv_Thickness = i.uv_texcoord * _Thickness_ST.xy + _Thickness_ST.zw;
			float clampResult349 = clamp( (_ThicknessRemapMin + (( 1.0 - SAMPLE_TEXTURE2D( _Thickness, sampler_Linear_Repeat, uv_Thickness ).r ) - 0.0) * (_ThicknessRemapMax - _ThicknessRemapMin) / (1.0 - 0.0)) , 0.0 , 1.0 );
			o.Emission = ( clampResult349 * ( tex2DNode306 * _ThicknessEmission ) ).rgb;
			float2 uv_BaseMaskMap = i.uv_texcoord * _BaseMaskMap_ST.xy + _BaseMaskMap_ST.zw;
			float4 tex2DNode308 = SAMPLE_TEXTURE2D( _BaseMaskMap, sampler_Linear_Repeat, uv_BaseMaskMap );
			o.Metallic = ( _Metallic * tex2DNode308.r );
			o.Smoothness = (_BaseSmoothnessRemapMin + (tex2DNode308.a - 0.0) * (_BaseSmoothnessRemapMax - _BaseSmoothnessRemapMin) / (1.0 - 0.0));
			o.Occlusion = (_BaseAORemapMin + (tex2DNode308.g - 0.0) * (_BaseAORemapMax - _BaseAORemapMin) / (1.0 - 0.0));
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}