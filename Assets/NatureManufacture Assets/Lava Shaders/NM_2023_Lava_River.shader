Shader "NatureManufacture/Lava River/Lava River"
{
	Properties
	{
		_GlobalTiling("Global Tiling", Float) = 1
		[Toggle(_UVVDIRECTION1UDIRECTION0_ON)] _UVVDirection1UDirection0("UV Direction - V(T) U(F)", Float) = 0
		_ColdLavaMainSpeed("Cold Lava Main Speed", Vector) = (1,1,0,0)
		_ColdLavaFlowUVRefresSpeed("Cold Lava Flow UV Refresh Speed", Range( 0 , 1)) = 0.05
		_MediumLavaMainSpeed("Medium Lava Main Speed", Vector) = (1,1,0,0)
		_MediumLavaFlowUVRefreshSpeed("Medium Lava Flow UV Refresh Speed", Range( 0 , 1)) = 0.05
		_HotLavaMainSpeed("Hot Lava Main Speed", Vector) = (1,1,0,0)
		_HotLavaFlowUVRefreshSpeed("Hot Lava Flow UV Refresh Speed", Range( 0 , 1)) = 0.05
		_Slope_Speed_Influence("Slope Speed Influence", Vector) = (1,1,0,0)
		_ColdLavaAlbedo_SM("Cold Lava Albedo_SM", 2D) = "white" {}
		_ColdLavaAlbedoColor("Cold Lava Albedo Color", Color) = (1,1,1,0)
		_ColdLavaAlbedoColorMultiply("Cold Lava Albedo Color Multiply ", Float) = 1
		_ColdLavaTiling("Cold Lava Tiling", Vector) = (1,1,0,0)
		_ColdLavaSmoothness("Cold Lava Smoothness", Range( 0 , 1)) = 1
		_ColdLavaNormal("Cold Lava Normal", 2D) = "bump" {}
		_ColdLavaNormalScale("Cold Lava Normal Scale", Float) = 1
		_ColdLavaMT_AO_H_EM("Cold Lava MT_AO_H_EM", 2D) = "white" {}
		_ColdLavaMetalic("Cold Lava Metalic", Range( 0 , 1)) = 1
		_ColdLavaAO("Cold Lava AO", Range( 0 , 1)) = 1
		_MediumLavaAngle("Medium Lava Angle", Range( 0.001 , 90)) = 4
		_MediumLavaAngleFalloff("Medium Lava Angle Falloff", Range( 0 , 80)) = 0.7
		_MediumLavaHeightBlendTreshold("Medium Lava Height Blend Treshold", Range( 0 , 10)) = 3.76
		_MediumLavaHeightBlendStrenght("Medium Lava Height Blend Strenght", Range( 0 , 20)) = 2.75
		_MediumLavaAlbedo_SM("Medium Lava Albedo_SM", 2D) = "white" {}
		_MediumLavaAlbedoColor("Medium Lava Albedo Color", Color) = (1,1,1,0)
		_MediumLavaAlbedoColorMultiply("Medium Lava Albedo Color Multiply ", Float) = 1
		_MediumLavaTiling("Medium Lava Tiling", Vector) = (1,1,0,0)
		_MediumLavaSmoothness("Medium Lava Smoothness", Range( 0 , 1)) = 1
		_MediumLavaNormal("Medium Lava Normal", 2D) = "bump" {}
		_MediumLavaNormalScale("Medium Lava Normal Scale", Float) = 1
		_MediumLavaMT_AO_H_EM("Medium Lava MT_AO_H_EM", 2D) = "white" {}
		_MediumLavaMetallic("Medium Lava Metallic", Range( 0 , 1)) = 1
		_MediumLavaAO("Medium Lava AO", Range( 0 , 1)) = 1
		_HotLavaAngle("Hot Lava Angle", Range( 0.001 , 90)) = 9.8
		_HotLavaAngleFalloff("Hot Lava Angle Falloff", Range( 0 , 80)) = 1.5
		_HotLavaHeightBlendTreshold("Hot Lava Height Blend Treshold", Range( 0 , 10)) = 3.09
		_HotLavaHeightBlendStrenght("Hot Lava Height Blend Strenght", Range( 0 , 20)) = 2.75
		_HotLavaAlbedo_SM("Hot Lava Albedo_SM", 2D) = "white" {}
		_HotLavaAlbedoColor("Hot Lava Albedo Color", Color) = (1,1,1,0)
		_HotLavaAlbedoColorMultiply("Hot Lava Albedo Color Multiply ", Float) = 1
		_HotLavaTiling("Hot Lava Tiling", Vector) = (1,1,0,0)
		_HotLavaSmoothness("Hot Lava Smoothness", Range( 0 , 1)) = 1
		_HotLavaNormal("Hot Lava Normal", 2D) = "bump" {}
		_HotLavaNormalScale("Hot Lava Normal Scale", Float) = 1
		_HotLavaMT_AO_H_EM("Hot Lava MT_AO_H_EM", 2D) = "white" {}
		_HotLavaMetallic("Hot Lava Metallic", Range( 0 , 1)) = 1
		_HotLavaAO("Hot Lava AO", Range( 0 , 1)) = 1
		[HDR]_LavaEmissionColor("Lava Emission Color", Color) = (1,0.1862055,0,0)
		_ColdLavaEmissionMaskIntensivity("Cold Lava Emission Mask Intensivity", Range( 0 , 100)) = 1.9
		_ColdLavaEmissionMaskTreshold("Cold Lava Emission Mask Treshold", Float) = 2.55
		_MediumLavaEmissionMaskIntesivity("Medium Lava Emission Mask Intesivity", Range( 0 , 100)) = 3.8
		_MediumLavaEmissionMaskTreshold("Medium Lava Emission Mask Treshold", Float) = 3.15
		_HotLavaEmissionMaskIntensivity("Hot Lava Emission Mask Intensivity", Range( 0 , 100)) = 2
		_HotLavaEmissionMaskTreshold("Hot Lava Emission Mask Treshold", Float) = 9.52
		[HDR]_RimColor("Rim Color", Color) = (1,0,0,0)
		_RimLightPower("Rim Light Power", Float) = 4
		_Noise("Noise", 2D) = "white" {}
		_NoiseTiling("Noise Tiling", Vector) = (1,1,0,0)
		_NoiseSpeed("Noise Speed", Vector) = (0.5,0.5,0,0)
		_HotLavaFlowUVRefreshSpeed_1("Noise Flow UV Refresh Speed", Range( 0 , 1)) = 0.05
		_ColdLavaNoisePower("Cold Lava Noise Power", Range( 0 , 10)) = 6.45
		_MediumLavaNoisePower("Medium Lava Noise Power", Range( 0 , 10)) = 2.47
		_HotLavaNoisePower("Hot Lava Noise Power", Range( 0 , 10)) = 5.48
		_VCColdLavaHeightBlendStrenght("VC Cold Lava Height Blend Strenght", Range( 0 , 10)) = 0
		_VCMediumLavaHeightBlendStrenght("VC Medium Lava Height Blend Strenght", Range( 0 , 10)) = 0
		_VCHotLavaHeightBlendStrenght("VC Hot Lava Height Blend Strenght", Range( 0 , 10)) = 0
		[Toggle(_Dynamic_Flow)] _Dynamic_Flow("Dynamic Lava Flow", Float) = 0
		_Dynamic_Start_Position_Offset("Dynamic Start Position Offset", Float) = 0
		_Dynamic_Shape_Speed("Dynamic Shape Speed", Range( 0 , 10)) = 0.1
		_Dynamic_Shape_Y_Offset("Dynamic Shape Y Offset", Float) = 0
		_Dynamic_Shape_U_Curve_Power("Dynamic Shape U Curve Power", Range( -8 , 8)) = 2.3
		_Dynamic_Shape_V_Curve_Power("Dynamic Shape V Curve Power", Range( -8 , 8)) = 1.5
		_Dynamic_Lava_Emission_Front_Mask_Intensivity("Dynamic Lava Emission Front Mask Intensivity", Float) = 2.2
		_Dynamic_Lava_Emission_Front_Mask_Treshold("Dynamic Lava Emission Front Mask Treshold", Float) = 2.2
		_Dynamic_Lava_Emission_Intensivity("Dynamic Lava Emission Intensivity", Float) = 2.2
		_Dynamic_Lava_Emission_Treshold("Dynamic Lava Emission Treshold", Float) = 2.2
		_ColdLavaTessScale("Cold Lava Tess Scale", Float) = 0.05
		_MediumLavaTessScale("Medium Lava Tess Scale", Float) = 0.15
		_HotLavaTessScale("Hot Lava Tess Scale", Float) = 0.3
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _Dynamic_Flow
		#pragma shader_feature_local _UVVDIRECTION1UDIRECTION0_ON
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
		#endif//ASE Sampling Macros

		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv4_texcoord4;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float2 uv3_texcoord3;
			float3 viewDir;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_ColdLavaMT_AO_H_EM);
		uniform float2 _Slope_Speed_Influence;
		uniform float2 _ColdLavaMainSpeed;
		uniform float2 _ColdLavaTiling;
		uniform float _ColdLavaFlowUVRefresSpeed;
		uniform float _GlobalTiling;
		SamplerState sampler_Linear_Repeat_Aniso8;
		SamplerState sampler_ColdLavaMT_AO_H_EM;
		uniform float _ColdLavaTessScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MediumLavaMT_AO_H_EM);
		uniform float2 _MediumLavaMainSpeed;
		uniform float2 _MediumLavaTiling;
		uniform float _MediumLavaFlowUVRefreshSpeed;
		SamplerState sampler_MediumLavaMT_AO_H_EM;
		uniform float _MediumLavaTessScale;
		uniform float _MediumLavaHeightBlendTreshold;
		uniform float _MediumLavaAngle;
		uniform float _MediumLavaAngleFalloff;
		uniform float _MediumLavaHeightBlendStrenght;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_HotLavaMT_AO_H_EM);
		uniform float2 _HotLavaMainSpeed;
		uniform float2 _HotLavaTiling;
		uniform float _HotLavaFlowUVRefreshSpeed;
		SamplerState sampler_HotLavaMT_AO_H_EM;
		uniform float _HotLavaTessScale;
		uniform float _HotLavaHeightBlendTreshold;
		uniform float _HotLavaAngle;
		uniform float _HotLavaAngleFalloff;
		uniform float _HotLavaHeightBlendStrenght;
		uniform float _VCColdLavaHeightBlendStrenght;
		uniform float _VCMediumLavaHeightBlendStrenght;
		uniform float _VCHotLavaHeightBlendStrenght;
		uniform float _Dynamic_Start_Position_Offset;
		uniform float _Dynamic_Shape_Speed;
		uniform float _Dynamic_Shape_U_Curve_Power;
		uniform float _Dynamic_Shape_V_Curve_Power;
		uniform float _Dynamic_Shape_Y_Offset;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_ColdLavaNormal);
		uniform float _ColdLavaNormalScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MediumLavaNormal);
		uniform float _MediumLavaNormalScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_HotLavaNormal);
		uniform float _HotLavaNormalScale;
		uniform float4 _ColdLavaAlbedoColor;
		uniform float _ColdLavaAlbedoColorMultiply;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_ColdLavaAlbedo_SM);
		uniform float _ColdLavaSmoothness;
		uniform float4 _MediumLavaAlbedoColor;
		uniform float _MediumLavaAlbedoColorMultiply;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MediumLavaAlbedo_SM);
		uniform float _MediumLavaSmoothness;
		uniform float4 _HotLavaAlbedoColor;
		uniform float _HotLavaAlbedoColorMultiply;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_HotLavaAlbedo_SM);
		uniform float _HotLavaSmoothness;
		uniform float _ColdLavaMetalic;
		uniform float _ColdLavaAO;
		uniform float _ColdLavaEmissionMaskIntensivity;
		uniform float _ColdLavaEmissionMaskTreshold;
		uniform float _MediumLavaMetallic;
		uniform float _MediumLavaAO;
		uniform float _MediumLavaEmissionMaskIntesivity;
		uniform float _MediumLavaEmissionMaskTreshold;
		uniform float _HotLavaMetallic;
		uniform float _HotLavaAO;
		uniform float _HotLavaEmissionMaskIntensivity;
		uniform float _HotLavaEmissionMaskTreshold;
		uniform float _Dynamic_Lava_Emission_Front_Mask_Intensivity;
		uniform float _Dynamic_Lava_Emission_Front_Mask_Treshold;
		uniform float _Dynamic_Lava_Emission_Intensivity;
		uniform float _Dynamic_Lava_Emission_Treshold;
		uniform float4 _LavaEmissionColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Noise);
		uniform float2 _NoiseSpeed;
		uniform float2 _NoiseTiling;
		uniform float _HotLavaFlowUVRefreshSpeed_1;
		SamplerState sampler_Linear_Repeat;
		uniform float _ColdLavaNoisePower;
		uniform float _MediumLavaNoisePower;
		uniform float _HotLavaNoisePower;
		uniform float4 _RimColor;
		uniform float _RimLightPower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 clampResult329 = clamp( ase_vertexNormal , float3( 0,0,0 ) , float3( 1,1,1 ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float clampResult44_g28 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 SlopeSpeedInfluence18 = _Slope_Speed_Influence;
			float2 temp_output_66_0_g28 = _ColdLavaTiling;
			float2 temp_output_53_0_g28 = ( ( ( ( ( 1.0 - clampResult44_g28 ) * SlopeSpeedInfluence18 ) + _ColdLavaMainSpeed ) * temp_output_66_0_g28 ) * v.texcoord3.xy );
			float2 break56_g28 = temp_output_53_0_g28;
			float2 appendResult57_g28 = (float2(break56_g28.y , break56_g28.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g28 = temp_output_53_0_g28;
			#else
				float2 staticSwitch59_g28 = appendResult57_g28;
			#endif
			float temp_output_68_0_g28 = ( _Time.y * _ColdLavaFlowUVRefresSpeed );
			float temp_output_71_0_g28 = frac( ( temp_output_68_0_g28 + 0.0 ) );
			float2 temp_output_60_0_g28 = ( staticSwitch59_g28 * temp_output_71_0_g28 );
			float GlobalTiling21 = _GlobalTiling;
			float2 temp_output_83_0_g28 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g28 * v.texcoord.xy ) );
			float2 temp_output_86_0_g28 = ( temp_output_60_0_g28 + temp_output_83_0_g28 );
			float2 temp_output_327_91 = temp_output_86_0_g28;
			float4 tex2DNode41 = SAMPLE_TEXTURE2D_LOD( _ColdLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_327_91, 0.0 );
			float3 appendResult49 = (float3(tex2DNode41.rgb));
			float2 temp_output_80_0_g28 = ( staticSwitch59_g28 * frac( ( temp_output_68_0_g28 + -0.5 ) ) );
			float2 temp_output_327_93 = ( temp_output_83_0_g28 + temp_output_80_0_g28 );
			float4 tex2DNode42 = SAMPLE_TEXTURE2D_LOD( _ColdLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_327_93, 0.0 );
			float3 appendResult50 = (float3(tex2DNode42.rgb));
			float clampResult90_g28 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g28 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g28 = clamp( pow( clampResult90_g28 , ( SAMPLE_TEXTURE2D_LOD( _ColdLavaMT_AO_H_EM, sampler_ColdLavaMT_AO_H_EM, temp_output_86_0_g28, 0.0 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_327_98 = clampResult104_g28;
			float3 lerpResult46 = lerp( appendResult49 , appendResult50 , temp_output_327_98);
			float3 break80 = lerpResult46;
			float temp_output_312_0 = ( ( break80.z + -0.25 ) * _ColdLavaTessScale );
			float clampResult44_g26 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 temp_output_66_0_g26 = _MediumLavaTiling;
			float2 temp_output_53_0_g26 = ( ( ( ( ( 1.0 - clampResult44_g26 ) * SlopeSpeedInfluence18 ) + _MediumLavaMainSpeed ) * temp_output_66_0_g26 ) * v.texcoord3.xy );
			float2 break56_g26 = temp_output_53_0_g26;
			float2 appendResult57_g26 = (float2(break56_g26.y , break56_g26.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g26 = temp_output_53_0_g26;
			#else
				float2 staticSwitch59_g26 = appendResult57_g26;
			#endif
			float temp_output_68_0_g26 = ( _Time.y * _MediumLavaFlowUVRefreshSpeed );
			float temp_output_71_0_g26 = frac( ( temp_output_68_0_g26 + 0.0 ) );
			float2 temp_output_60_0_g26 = ( staticSwitch59_g26 * temp_output_71_0_g26 );
			float2 temp_output_83_0_g26 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g26 * v.texcoord.xy ) );
			float2 temp_output_86_0_g26 = ( temp_output_60_0_g26 + temp_output_83_0_g26 );
			float2 temp_output_325_91 = temp_output_86_0_g26;
			float4 tex2DNode118 = SAMPLE_TEXTURE2D_LOD( _MediumLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_325_91, 0.0 );
			float3 appendResult123 = (float3(tex2DNode118.rgb));
			float2 temp_output_80_0_g26 = ( staticSwitch59_g26 * frac( ( temp_output_68_0_g26 + -0.5 ) ) );
			float2 temp_output_325_93 = ( temp_output_83_0_g26 + temp_output_80_0_g26 );
			float4 tex2DNode119 = SAMPLE_TEXTURE2D_LOD( _MediumLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_325_93, 0.0 );
			float3 appendResult124 = (float3(tex2DNode119.rgb));
			float clampResult90_g26 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g26 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g26 = clamp( pow( clampResult90_g26 , ( SAMPLE_TEXTURE2D_LOD( _MediumLavaMT_AO_H_EM, sampler_MediumLavaMT_AO_H_EM, temp_output_86_0_g26, 0.0 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_325_98 = clampResult104_g26;
			float3 lerpResult122 = lerp( appendResult123 , appendResult124 , temp_output_325_98);
			float3 break125 = lerpResult122;
			float temp_output_320_0 = ( ( break125.z + -0.25 ) * _MediumLavaTessScale );
			float temp_output_106_0 = ( 1.0 - break80.z );
			float clampResult53 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float temp_output_56_0 = ( _MediumLavaAngle / 45.0 );
			float clampResult64 = clamp( ( clampResult53 - ( 1.0 - temp_output_56_0 ) ) , 0.0 , 2.0 );
			float clampResult66 = clamp( ( clampResult64 * ( 1.0 / temp_output_56_0 ) ) , 0.0 , 1.0 );
			float clampResult75 = clamp( pow( abs( ( 1.0 - clampResult66 ) ) , _MediumLavaAngleFalloff ) , 0.0 , 1.0 );
			float temp_output_67_0_g12 = clampResult75;
			float temp_output_112_98 = saturate( pow( abs( ( ( ( pow( abs( temp_output_106_0 ) , _MediumLavaHeightBlendTreshold ) * temp_output_67_0_g12 ) * 4.0 ) + ( temp_output_67_0_g12 * 2.0 ) ) ) , _MediumLavaHeightBlendStrenght ) );
			float lerpResult306 = lerp( temp_output_312_0 , temp_output_320_0 , temp_output_112_98);
			float clampResult44_g27 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 temp_output_66_0_g27 = _HotLavaTiling;
			float2 temp_output_53_0_g27 = ( ( ( ( ( 1.0 - clampResult44_g27 ) * SlopeSpeedInfluence18 ) + _HotLavaMainSpeed ) * temp_output_66_0_g27 ) * v.texcoord3.xy );
			float2 break56_g27 = temp_output_53_0_g27;
			float2 appendResult57_g27 = (float2(break56_g27.y , break56_g27.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g27 = temp_output_53_0_g27;
			#else
				float2 staticSwitch59_g27 = appendResult57_g27;
			#endif
			float temp_output_68_0_g27 = ( _Time.y * _HotLavaFlowUVRefreshSpeed );
			float temp_output_71_0_g27 = frac( ( temp_output_68_0_g27 + 0.0 ) );
			float2 temp_output_60_0_g27 = ( staticSwitch59_g27 * temp_output_71_0_g27 );
			float2 temp_output_83_0_g27 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g27 * v.texcoord.xy ) );
			float2 temp_output_86_0_g27 = ( temp_output_60_0_g27 + temp_output_83_0_g27 );
			float2 temp_output_326_91 = temp_output_86_0_g27;
			float4 tex2DNode165 = SAMPLE_TEXTURE2D_LOD( _HotLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_326_91, 0.0 );
			float3 appendResult169 = (float3(tex2DNode165.rgb));
			float2 temp_output_80_0_g27 = ( staticSwitch59_g27 * frac( ( temp_output_68_0_g27 + -0.5 ) ) );
			float2 temp_output_326_93 = ( temp_output_83_0_g27 + temp_output_80_0_g27 );
			float4 tex2DNode166 = SAMPLE_TEXTURE2D_LOD( _HotLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_326_93, 0.0 );
			float3 appendResult170 = (float3(tex2DNode166.rgb));
			float clampResult90_g27 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g27 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g27 = clamp( pow( clampResult90_g27 , ( SAMPLE_TEXTURE2D_LOD( _HotLavaMT_AO_H_EM, sampler_HotLavaMT_AO_H_EM, temp_output_86_0_g27, 0.0 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_326_98 = clampResult104_g27;
			float3 lerpResult168 = lerp( appendResult169 , appendResult170 , temp_output_326_98);
			float3 break171 = lerpResult168;
			float temp_output_323_0 = ( ( break171.z + -0.25 ) * _HotLavaTessScale );
			float temp_output_155_0 = ( 1.0 - break125.z );
			float temp_output_59_0 = ( _HotLavaAngle / 45.0 );
			float clampResult65 = clamp( ( clampResult53 - ( 1.0 - temp_output_59_0 ) ) , 0.0 , 2.0 );
			float clampResult70 = clamp( ( clampResult65 * ( 1.0 / temp_output_59_0 ) ) , 0.0 , 1.0 );
			float clampResult74 = clamp( pow( abs( ( 1.0 - clampResult70 ) ) , _HotLavaAngleFalloff ) , 0.0 , 1.0 );
			float temp_output_67_0_g11 = clampResult74;
			float temp_output_158_98 = saturate( pow( abs( ( ( ( pow( abs( temp_output_155_0 ) , _HotLavaHeightBlendTreshold ) * temp_output_67_0_g11 ) * 4.0 ) + ( temp_output_67_0_g11 * 2.0 ) ) ) , _HotLavaHeightBlendStrenght ) );
			float lerpResult307 = lerp( lerpResult306 , temp_output_323_0 , temp_output_158_98);
			float4 clampResult214 = clamp( v.color , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 break215 = clampResult214;
			float temp_output_67_0_g13 = break215.r;
			float temp_output_209_98 = saturate( pow( abs( ( ( ( temp_output_106_0 * temp_output_67_0_g13 ) * 4.0 ) + ( temp_output_67_0_g13 * 2.0 ) ) ) , _VCColdLavaHeightBlendStrenght ) );
			float lerpResult309 = lerp( lerpResult307 , temp_output_312_0 , temp_output_209_98);
			float temp_output_67_0_g14 = break215.g;
			float temp_output_210_98 = saturate( pow( abs( ( ( ( temp_output_155_0 * temp_output_67_0_g14 ) * 4.0 ) + ( temp_output_67_0_g14 * 2.0 ) ) ) , _VCMediumLavaHeightBlendStrenght ) );
			float lerpResult308 = lerp( lerpResult309 , temp_output_320_0 , temp_output_210_98);
			float temp_output_67_0_g15 = break215.b;
			float temp_output_211_98 = saturate( pow( abs( ( ( ( ( 1.0 - break171.z ) * temp_output_67_0_g15 ) * 4.0 ) + ( temp_output_67_0_g15 * 2.0 ) ) ) , _VCHotLavaHeightBlendStrenght ) );
			float lerpResult310 = lerp( lerpResult308 , temp_output_323_0 , temp_output_211_98);
			float3 temp_output_302_0 = ( clampResult329 * ase_vertexNormal * lerpResult310 );
			float temp_output_124_0_g18 = ( _Dynamic_Start_Position_Offset + ( _Time.y * _Dynamic_Shape_Speed ) );
			float temp_output_126_0_g18 = ( temp_output_124_0_g18 + _Dynamic_Shape_U_Curve_Power );
			float temp_output_115_0_g18 = ( v.texcoord2.xy.x + ( ( 1.0 - sin( ( v.texcoord2.xy.y * UNITY_PI ) ) ) * _Dynamic_Shape_V_Curve_Power ) );
			float smoothstepResult125_g18 = smoothstep( temp_output_124_0_g18 , temp_output_126_0_g18 , temp_output_115_0_g18);
			float3 appendResult131_g18 = (float3(0.0 , ( smoothstepResult125_g18 * _Dynamic_Shape_Y_Offset ) , 0.0));
			float3 clampResult330 = clamp( ase_vertexNormal , float3( 0,0,0 ) , float3( 1,1,1 ) );
			#ifdef _Dynamic_Flow
				float3 staticSwitch237 = ( ( appendResult131_g18 * clampResult330 ) + temp_output_302_0 );
			#else
				float3 staticSwitch237 = temp_output_302_0;
			#endif
			v.vertex.xyz += staticSwitch237;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult44_g28 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 SlopeSpeedInfluence18 = _Slope_Speed_Influence;
			float2 temp_output_66_0_g28 = _ColdLavaTiling;
			float2 temp_output_53_0_g28 = ( ( ( ( ( 1.0 - clampResult44_g28 ) * SlopeSpeedInfluence18 ) + _ColdLavaMainSpeed ) * temp_output_66_0_g28 ) * i.uv4_texcoord4 );
			float2 break56_g28 = temp_output_53_0_g28;
			float2 appendResult57_g28 = (float2(break56_g28.y , break56_g28.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g28 = temp_output_53_0_g28;
			#else
				float2 staticSwitch59_g28 = appendResult57_g28;
			#endif
			float temp_output_68_0_g28 = ( _Time.y * _ColdLavaFlowUVRefresSpeed );
			float temp_output_71_0_g28 = frac( ( temp_output_68_0_g28 + 0.0 ) );
			float2 temp_output_60_0_g28 = ( staticSwitch59_g28 * temp_output_71_0_g28 );
			float GlobalTiling21 = _GlobalTiling;
			float2 temp_output_83_0_g28 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g28 * i.uv_texcoord ) );
			float2 temp_output_86_0_g28 = ( temp_output_60_0_g28 + temp_output_83_0_g28 );
			float2 temp_output_327_91 = temp_output_86_0_g28;
			float2 temp_output_80_0_g28 = ( staticSwitch59_g28 * frac( ( temp_output_68_0_g28 + -0.5 ) ) );
			float2 temp_output_327_93 = ( temp_output_83_0_g28 + temp_output_80_0_g28 );
			float clampResult90_g28 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g28 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g28 = clamp( pow( clampResult90_g28 , ( SAMPLE_TEXTURE2D( _ColdLavaMT_AO_H_EM, sampler_ColdLavaMT_AO_H_EM, temp_output_86_0_g28 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_327_98 = clampResult104_g28;
			float3 lerpResult45 = lerp( UnpackScaleNormal( SAMPLE_TEXTURE2D( _ColdLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_327_91 ), _ColdLavaNormalScale ) , UnpackScaleNormal( SAMPLE_TEXTURE2D( _ColdLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_327_93 ), _ColdLavaNormalScale ) , temp_output_327_98);
			float clampResult44_g26 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 temp_output_66_0_g26 = _MediumLavaTiling;
			float2 temp_output_53_0_g26 = ( ( ( ( ( 1.0 - clampResult44_g26 ) * SlopeSpeedInfluence18 ) + _MediumLavaMainSpeed ) * temp_output_66_0_g26 ) * i.uv4_texcoord4 );
			float2 break56_g26 = temp_output_53_0_g26;
			float2 appendResult57_g26 = (float2(break56_g26.y , break56_g26.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g26 = temp_output_53_0_g26;
			#else
				float2 staticSwitch59_g26 = appendResult57_g26;
			#endif
			float temp_output_68_0_g26 = ( _Time.y * _MediumLavaFlowUVRefreshSpeed );
			float temp_output_71_0_g26 = frac( ( temp_output_68_0_g26 + 0.0 ) );
			float2 temp_output_60_0_g26 = ( staticSwitch59_g26 * temp_output_71_0_g26 );
			float2 temp_output_83_0_g26 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g26 * i.uv_texcoord ) );
			float2 temp_output_86_0_g26 = ( temp_output_60_0_g26 + temp_output_83_0_g26 );
			float2 temp_output_325_91 = temp_output_86_0_g26;
			float2 temp_output_80_0_g26 = ( staticSwitch59_g26 * frac( ( temp_output_68_0_g26 + -0.5 ) ) );
			float2 temp_output_325_93 = ( temp_output_83_0_g26 + temp_output_80_0_g26 );
			float clampResult90_g26 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g26 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g26 = clamp( pow( clampResult90_g26 , ( SAMPLE_TEXTURE2D( _MediumLavaMT_AO_H_EM, sampler_MediumLavaMT_AO_H_EM, temp_output_86_0_g26 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_325_98 = clampResult104_g26;
			float3 lerpResult120 = lerp( UnpackScaleNormal( SAMPLE_TEXTURE2D( _MediumLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_325_91 ), _MediumLavaNormalScale ) , UnpackScaleNormal( SAMPLE_TEXTURE2D( _MediumLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_325_93 ), _MediumLavaNormalScale ) , temp_output_325_98);
			float4 tex2DNode41 = SAMPLE_TEXTURE2D( _ColdLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_327_91 );
			float3 appendResult49 = (float3(tex2DNode41.rgb));
			float4 tex2DNode42 = SAMPLE_TEXTURE2D( _ColdLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_327_93 );
			float3 appendResult50 = (float3(tex2DNode42.rgb));
			float3 lerpResult46 = lerp( appendResult49 , appendResult50 , temp_output_327_98);
			float3 break80 = lerpResult46;
			float temp_output_106_0 = ( 1.0 - break80.z );
			float clampResult53 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float temp_output_56_0 = ( _MediumLavaAngle / 45.0 );
			float clampResult64 = clamp( ( clampResult53 - ( 1.0 - temp_output_56_0 ) ) , 0.0 , 2.0 );
			float clampResult66 = clamp( ( clampResult64 * ( 1.0 / temp_output_56_0 ) ) , 0.0 , 1.0 );
			float clampResult75 = clamp( pow( abs( ( 1.0 - clampResult66 ) ) , _MediumLavaAngleFalloff ) , 0.0 , 1.0 );
			float temp_output_67_0_g12 = clampResult75;
			float temp_output_112_98 = saturate( pow( abs( ( ( ( pow( abs( temp_output_106_0 ) , _MediumLavaHeightBlendTreshold ) * temp_output_67_0_g12 ) * 4.0 ) + ( temp_output_67_0_g12 * 2.0 ) ) ) , _MediumLavaHeightBlendStrenght ) );
			float3 lerpResult152 = lerp( lerpResult45 , lerpResult120 , temp_output_112_98);
			float clampResult44_g27 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 temp_output_66_0_g27 = _HotLavaTiling;
			float2 temp_output_53_0_g27 = ( ( ( ( ( 1.0 - clampResult44_g27 ) * SlopeSpeedInfluence18 ) + _HotLavaMainSpeed ) * temp_output_66_0_g27 ) * i.uv4_texcoord4 );
			float2 break56_g27 = temp_output_53_0_g27;
			float2 appendResult57_g27 = (float2(break56_g27.y , break56_g27.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g27 = temp_output_53_0_g27;
			#else
				float2 staticSwitch59_g27 = appendResult57_g27;
			#endif
			float temp_output_68_0_g27 = ( _Time.y * _HotLavaFlowUVRefreshSpeed );
			float temp_output_71_0_g27 = frac( ( temp_output_68_0_g27 + 0.0 ) );
			float2 temp_output_60_0_g27 = ( staticSwitch59_g27 * temp_output_71_0_g27 );
			float2 temp_output_83_0_g27 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g27 * i.uv_texcoord ) );
			float2 temp_output_86_0_g27 = ( temp_output_60_0_g27 + temp_output_83_0_g27 );
			float2 temp_output_326_91 = temp_output_86_0_g27;
			float2 temp_output_80_0_g27 = ( staticSwitch59_g27 * frac( ( temp_output_68_0_g27 + -0.5 ) ) );
			float2 temp_output_326_93 = ( temp_output_83_0_g27 + temp_output_80_0_g27 );
			float clampResult90_g27 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g27 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float clampResult104_g27 = clamp( pow( clampResult90_g27 , ( SAMPLE_TEXTURE2D( _HotLavaMT_AO_H_EM, sampler_HotLavaMT_AO_H_EM, temp_output_86_0_g27 ).b * 7.0 ) ) , 0.0 , 1.0 );
			float temp_output_326_98 = clampResult104_g27;
			float3 lerpResult187 = lerp( UnpackScaleNormal( SAMPLE_TEXTURE2D( _HotLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_326_91 ), _HotLavaNormalScale ) , UnpackScaleNormal( SAMPLE_TEXTURE2D( _HotLavaNormal, sampler_Linear_Repeat_Aniso8, temp_output_326_93 ), _HotLavaNormalScale ) , temp_output_326_98);
			float4 tex2DNode118 = SAMPLE_TEXTURE2D( _MediumLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_325_91 );
			float3 appendResult123 = (float3(tex2DNode118.rgb));
			float4 tex2DNode119 = SAMPLE_TEXTURE2D( _MediumLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_325_93 );
			float3 appendResult124 = (float3(tex2DNode119.rgb));
			float3 lerpResult122 = lerp( appendResult123 , appendResult124 , temp_output_325_98);
			float3 break125 = lerpResult122;
			float temp_output_155_0 = ( 1.0 - break125.z );
			float temp_output_59_0 = ( _HotLavaAngle / 45.0 );
			float clampResult65 = clamp( ( clampResult53 - ( 1.0 - temp_output_59_0 ) ) , 0.0 , 2.0 );
			float clampResult70 = clamp( ( clampResult65 * ( 1.0 / temp_output_59_0 ) ) , 0.0 , 1.0 );
			float clampResult74 = clamp( pow( abs( ( 1.0 - clampResult70 ) ) , _HotLavaAngleFalloff ) , 0.0 , 1.0 );
			float temp_output_67_0_g11 = clampResult74;
			float temp_output_158_98 = saturate( pow( abs( ( ( ( pow( abs( temp_output_155_0 ) , _HotLavaHeightBlendTreshold ) * temp_output_67_0_g11 ) * 4.0 ) + ( temp_output_67_0_g11 * 2.0 ) ) ) , _HotLavaHeightBlendStrenght ) );
			float3 lerpResult161 = lerp( lerpResult152 , lerpResult187 , temp_output_158_98);
			float4 clampResult214 = clamp( i.vertexColor , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 break215 = clampResult214;
			float temp_output_67_0_g13 = break215.r;
			float temp_output_209_98 = saturate( pow( abs( ( ( ( temp_output_106_0 * temp_output_67_0_g13 ) * 4.0 ) + ( temp_output_67_0_g13 * 2.0 ) ) ) , _VCColdLavaHeightBlendStrenght ) );
			float3 lerpResult200 = lerp( lerpResult161 , lerpResult45 , temp_output_209_98);
			float temp_output_67_0_g14 = break215.g;
			float temp_output_210_98 = saturate( pow( abs( ( ( ( temp_output_155_0 * temp_output_67_0_g14 ) * 4.0 ) + ( temp_output_67_0_g14 * 2.0 ) ) ) , _VCMediumLavaHeightBlendStrenght ) );
			float3 lerpResult205 = lerp( lerpResult200 , lerpResult120 , temp_output_210_98);
			float4 tex2DNode165 = SAMPLE_TEXTURE2D( _HotLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_326_91 );
			float3 appendResult169 = (float3(tex2DNode165.rgb));
			float4 tex2DNode166 = SAMPLE_TEXTURE2D( _HotLavaMT_AO_H_EM, sampler_Linear_Repeat_Aniso8, temp_output_326_93 );
			float3 appendResult170 = (float3(tex2DNode166.rgb));
			float3 lerpResult168 = lerp( appendResult169 , appendResult170 , temp_output_326_98);
			float3 break171 = lerpResult168;
			float temp_output_67_0_g15 = break215.b;
			float temp_output_211_98 = saturate( pow( abs( ( ( ( ( 1.0 - break171.z ) * temp_output_67_0_g15 ) * 4.0 ) + ( temp_output_67_0_g15 * 2.0 ) ) ) , _VCHotLavaHeightBlendStrenght ) );
			float3 lerpResult208 = lerp( lerpResult205 , lerpResult187 , temp_output_211_98);
			o.Normal = lerpResult208;
			float3 appendResult103 = (float3(_ColdLavaAlbedoColor.r , _ColdLavaAlbedoColor.g , _ColdLavaAlbedoColor.b));
			float4 lerpResult48 = lerp( SAMPLE_TEXTURE2D( _ColdLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_327_91 ) , SAMPLE_TEXTURE2D( _ColdLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_327_93 ) , temp_output_327_98);
			float4 break97 = lerpResult48;
			float3 appendResult100 = (float3(break97.r , break97.g , break97.b));
			float4 appendResult99 = (float4(( ( appendResult103 * _ColdLavaAlbedoColorMultiply ) * appendResult100 ) , ( break97.a * _ColdLavaSmoothness )));
			float3 appendResult145 = (float3(_MediumLavaAlbedoColor.r , _MediumLavaAlbedoColor.g , _MediumLavaAlbedoColor.b));
			float4 lerpResult137 = lerp( SAMPLE_TEXTURE2D( _MediumLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_325_91 ) , SAMPLE_TEXTURE2D( _MediumLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_325_93 ) , temp_output_325_98);
			float4 break140 = lerpResult137;
			float3 appendResult142 = (float3(break140.r , break140.g , break140.b));
			float4 appendResult147 = (float4(( ( appendResult145 * _MediumLavaAlbedoColorMultiply ) * appendResult142 ) , ( break140.a * _MediumLavaSmoothness )));
			float4 lerpResult154 = lerp( appendResult99 , appendResult147 , temp_output_112_98);
			float3 appendResult182 = (float3(_HotLavaAlbedoColor.r , _HotLavaAlbedoColor.g , _HotLavaAlbedoColor.b));
			float4 lerpResult177 = lerp( SAMPLE_TEXTURE2D( _HotLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_326_91 ) , SAMPLE_TEXTURE2D( _HotLavaAlbedo_SM, sampler_Linear_Repeat_Aniso8, temp_output_326_93 ) , temp_output_326_98);
			float4 break179 = lerpResult177;
			float3 appendResult181 = (float3(break179.r , break179.g , break179.b));
			float4 appendResult184 = (float4(( ( appendResult182 * _HotLavaAlbedoColorMultiply ) * appendResult181 ) , ( break179.a * _HotLavaSmoothness )));
			float4 lerpResult163 = lerp( lerpResult154 , appendResult184 , temp_output_158_98);
			float4 lerpResult202 = lerp( lerpResult163 , appendResult99 , temp_output_209_98);
			float4 lerpResult204 = lerp( lerpResult202 , appendResult147 , temp_output_210_98);
			float4 lerpResult207 = lerp( lerpResult204 , appendResult184 , temp_output_211_98);
			float3 appendResult220 = (float3(lerpResult207.xyz));
			o.Albedo = appendResult220;
			float clampResult90 = clamp( break80.y , ( 1.0 - _ColdLavaAO ) , 1.0 );
			float lerpResult47 = lerp( tex2DNode41.a , tex2DNode42.a , clampResult90_g28);
			float3 appendResult94 = (float3(( break80.x * _ColdLavaMetalic ) , clampResult90 , pow( abs( ( lerpResult47 * _ColdLavaEmissionMaskIntensivity ) ) , _ColdLavaEmissionMaskTreshold )));
			float clampResult133 = clamp( break125.y , ( 1.0 - _MediumLavaAO ) , 1.0 );
			float lerpResult121 = lerp( tex2DNode118.a , tex2DNode119.a , clampResult90_g26);
			float3 appendResult141 = (float3(( break125.x * _MediumLavaMetallic ) , clampResult133 , pow( abs( ( lerpResult121 * _MediumLavaEmissionMaskIntesivity ) ) , _MediumLavaEmissionMaskTreshold )));
			float3 lerpResult153 = lerp( appendResult94 , appendResult141 , temp_output_112_98);
			float clampResult175 = clamp( break171.y , ( 1.0 - _HotLavaAO ) , 1.0 );
			float lerpResult167 = lerp( tex2DNode165.a , tex2DNode166.a , clampResult90_g27);
			float temp_output_174_0 = pow( abs( ( lerpResult167 * _HotLavaEmissionMaskIntensivity ) ) , _HotLavaEmissionMaskTreshold );
			float3 appendResult180 = (float3(( break171.x * _HotLavaMetallic ) , clampResult175 , temp_output_174_0));
			float3 lerpResult162 = lerp( lerpResult153 , appendResult180 , temp_output_158_98);
			float3 lerpResult201 = lerp( lerpResult162 , appendResult94 , temp_output_209_98);
			float3 lerpResult203 = lerp( lerpResult201 , appendResult141 , temp_output_210_98);
			float3 lerpResult206 = lerp( lerpResult203 , appendResult180 , temp_output_211_98);
			float3 break221 = lerpResult206;
			float temp_output_124_0_g18 = ( _Dynamic_Start_Position_Offset + ( _Time.y * _Dynamic_Shape_Speed ) );
			float temp_output_126_0_g18 = ( temp_output_124_0_g18 + _Dynamic_Shape_U_Curve_Power );
			float temp_output_115_0_g18 = ( i.uv3_texcoord3.x + ( ( 1.0 - sin( ( i.uv3_texcoord3.y * UNITY_PI ) ) ) * _Dynamic_Shape_V_Curve_Power ) );
			float smoothstepResult125_g18 = smoothstep( temp_output_124_0_g18 , temp_output_126_0_g18 , temp_output_115_0_g18);
			float clampResult239 = clamp( frac( smoothstepResult125_g18 ) , 0.0 , 1.0 );
			float clampResult245 = clamp( pow( abs( ( clampResult239 * _Dynamic_Lava_Emission_Front_Mask_Intensivity ) ) , _Dynamic_Lava_Emission_Front_Mask_Treshold ) , 0.0 , 1.0 );
			float clampResult251 = clamp( ( break221.z * pow( abs( ( clampResult245 * _Dynamic_Lava_Emission_Intensivity ) ) , _Dynamic_Lava_Emission_Treshold ) ) , 0.0 , ( temp_output_174_0 * 2.0 ) );
			float clampResult132_g18 = clamp( ( temp_output_126_0_g18 - temp_output_115_0_g18 ) , 0.0 , 1.0 );
			#ifdef _Dynamic_Flow
				float staticSwitch256 = ( max( break221.z , clampResult251 ) * clampResult132_g18 );
			#else
				float staticSwitch256 = break221.z;
			#endif
			float clampResult44_g29 = clamp( abs( ase_worldNormal.y ) , 0.0 , 1.0 );
			float2 temp_output_66_0_g29 = _NoiseTiling;
			float2 temp_output_53_0_g29 = ( ( ( ( ( 1.0 - clampResult44_g29 ) * SlopeSpeedInfluence18 ) + _NoiseSpeed ) * temp_output_66_0_g29 ) * i.uv4_texcoord4 );
			float2 break56_g29 = temp_output_53_0_g29;
			float2 appendResult57_g29 = (float2(break56_g29.y , break56_g29.x));
			#ifdef _UVVDIRECTION1UDIRECTION0_ON
				float2 staticSwitch59_g29 = temp_output_53_0_g29;
			#else
				float2 staticSwitch59_g29 = appendResult57_g29;
			#endif
			float temp_output_68_0_g29 = ( _Time.y * _HotLavaFlowUVRefreshSpeed_1 );
			float temp_output_71_0_g29 = frac( ( temp_output_68_0_g29 + 0.0 ) );
			float2 temp_output_60_0_g29 = ( staticSwitch59_g29 * temp_output_71_0_g29 );
			float2 temp_output_83_0_g29 = ( ( 1.0 / GlobalTiling21 ) * ( temp_output_66_0_g29 * i.uv_texcoord ) );
			float2 temp_output_80_0_g29 = ( staticSwitch59_g29 * frac( ( temp_output_68_0_g29 + -0.5 ) ) );
			float clampResult90_g29 = clamp( abs( sin( ( ( UNITY_PI * 1.5 ) + ( temp_output_71_0_g29 * UNITY_PI ) ) ) ) , 0.0 , 1.0 );
			float lerpResult287 = lerp( SAMPLE_TEXTURE2D( _Noise, sampler_Linear_Repeat, ( temp_output_60_0_g29 + temp_output_83_0_g29 ) ).a , SAMPLE_TEXTURE2D( _Noise, sampler_Linear_Repeat, ( temp_output_83_0_g29 + temp_output_80_0_g29 ) ).a , clampResult90_g29);
			float lerpResult290 = lerp( _ColdLavaNoisePower , _MediumLavaNoisePower , clampResult75);
			float lerpResult291 = lerp( lerpResult290 , _HotLavaNoisePower , clampResult74);
			float clampResult296 = clamp( ( pow( abs( lerpResult287 ) , lerpResult291 ) * 20.0 ) , 0.05 , 1.2 );
			float3 normalizeResult266 = normalize( i.viewDir );
			float dotResult267 = dot( lerpResult208 , normalizeResult266 );
			float4 temp_output_259_0 = ( ( ( staticSwitch256 * _LavaEmissionColor ) * clampResult296 ) + ( staticSwitch256 * ( ( _RimColor * pow( abs( ( 1.0 - saturate( dotResult267 ) ) ) , 10.0 ) ) * _RimLightPower ) ) );
			float4 clampResult263 = clamp( temp_output_259_0 , float4( 0,0,0,0 ) , temp_output_259_0 );
			o.Emission = ( break215.a * clampResult263 ).rgb;
			o.Metallic = break221;
			o.Smoothness = lerpResult207.w;
			o.Occlusion = break221.y;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float2 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv4_texcoord4;
				o.customPack1.xy = v.texcoord3;
				o.customPack1.zw = customInputData.uv_texcoord;
				o.customPack1.zw = v.texcoord;
				o.customPack2.xy = customInputData.uv3_texcoord3;
				o.customPack2.xy = v.texcoord2;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv4_texcoord4 = IN.customPack1.xy;
				surfIN.uv_texcoord = IN.customPack1.zw;
				surfIN.uv3_texcoord3 = IN.customPack2.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}