Shader "NatureManufacture/URP/Lit/Top Cover Shape"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0
		[Toggle(_PLANARUV1_ON)] _PlanarUV1("PlanarUV", Float) = 0
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_BaseColorMap("Base Color", 2D) = "white" {}
		[Toggle]_BaseUsePlanarUV("Base Use Planar UV", Float) = 0
		[Toggle]_UV0_UV2("UV0 (T) UV2 (F)", Float) = 1
		_BaseTilingOffset("Base Tiling and Offset", Vector) = (1,1,0,0)
		_BaseNormalMap("Base Normal Map", 2D) = "bump" {}
		_BaseNormalScale("Base Normal Scale", Range( 0 , 8)) = 1
		_BaseMaskMap("Base Mask Map MT(R) AO(G) SM(A)", 2D) = "white" {}
		_BaseMetallic("Base Metallic", Range( 0 , 1)) = 1
		_BaseAORemapMin("Base AO Remap Min", Range( 0 , 1)) = 0
		_BaseAORemapMax("Base AO Remap Max", Range( 0 , 1)) = 1
		_BaseSmoothnessRemapMin("Base Smoothness Remap Min", Range( 0 , 1)) = 0
		_BaseSmoothnessRemapMax("Base Smoothness Remap Max", Range( 0 , 1)) = 1
		[Toggle(_USEDYNAMICCOVERTSTATICMASKF_ON)] _USEDYNAMICCOVERTSTATICMASKF("Use Dynamic Cover (T) Static Mask (F)", Float) = 1
		[Toggle(_USECOVERHEIGHTT_ON)] _USECoverHeightT("Use Cover Height (T)", Float) = 1
		_CoverMaskA("Cover Mask (A)", 2D) = "white" {}
		_CoverMaskPower("Cover Mask Power", Range( 0 , 10)) = 1
		_Cover_Amount("Cover Amount", Range( 0 , 2)) = 2
		_Cover_Amount_Grow_Speed("Cover Amount Grow Speed", Range( 0 , 3)) = 3
		_Cover_Max_Angle("Cover Max Angle", Range( 0.001 , 90)) = 35
		_CoverHardness("Cover Hardness", Range( 0 , 10)) = 5
		_Cover_Min_Height("Cover Min Height", Float) = -10000
		_Cover_Min_Height_Blending("Cover Min Height Blending", Range( 0 , 500)) = 1
		_CoverNormalBlendHardness("Cover Normal Blend Hardness", Range( 0 , 8)) = 1
		_Shape_Normal_Blend_Hardness("Shape Normal Blend Hardness", Range( 0 , 2)) = 1
		_VertexColorBBlendStrenght("Vertex Color (B) Blend Strenght", Range( 0 , 100)) = 10
		_VertexColorGBlendStrenght("Vertex Color (G) Blend Strenght", Range( 0 , 100)) = 10
		_CoverBaseColor("Cover Base Color", Color) = (1,1,1,1)
		_CoverBaseColorMap("Cover Base Map", 2D) = "white" {}
		[Toggle]_CoverUsePlanarUV("Cover Use Planar UV", Float) = 0
		[Toggle]_Cover_UV0_UV2("Cover UV0 (T) UV2 (F)", Float) = 1
		_CoverTilingOffset("Cover Tiling Offset", Vector) = (1,1,0,0)
		_CoverNormalMap("Cover Normal Map", 2D) = "bump" {}
		_CoverNormalScale("Cover Normal Scale", Range( 0 , 8)) = 1
		_CoverHeightMapMin("Cover Height Map Min", Float) = 0
		_CoverHeightMapMax("Cover Height Map Max", Float) = 1
		_CoverHeightMapOffset("Cover Height Map Offset", Float) = 0
		_CoverMaskMap("Cover Mask Map MT(R) AO(G) H(B) SM(A)", 2D) = "white" {}
		_CoverMetallic("Cover Metallic", Range( 0 , 1)) = 1
		_CoverAORemapMin("Cover AO Remap Min", Range( 0 , 1)) = 0
		_CoverAORemapMax("Cover AO Remap Max", Range( 0 , 1)) = 0
		_CoverSmoothnessRemapMin("Cover Smoothness Remap Min", Range( 0 , 1)) = 0
		_CoverSmoothnessRemapMax("Cover Smoothness Remap Max", Range( 0 , 1)) = 1
		[Toggle(_USE_SHAPEHEIGHTBT_STATIC_MASKF_ON)] _Use_ShapeHeightBT_Static_MaskF("Use Shape Height (B) (T) Cover Mask (F)", Float) = 0
		[Toggle]_Shape_UsePlanarUV("Shape Use Planar UV", Float) = 0
		[Toggle]_Shape_UV0_UV2("Shape UV0 (T) UV2 (F)", Float) = 1
		_ShapeTilingOffset("Shape Tiling and Offset", Vector) = (1,1,0,0)
		_ShapeNormal("Shape Normal", 2D) = "bump" {}
		_ShapeNormalStrenght1("Shape Normal Strenght Base", Range( 0 , 2)) = 1
		_ShapeNormalStrenght_1("Shape Normal Strenght Cover", Range( 0 , 2)) = 1
		_ShapeCurvAOHLeaksMask("Shape Curv (R) AO (G) H (B) Leaks Mask (A)", 2D) = "white" {}
		_CurvatureBlend("Curvature Power", Range( 0 , 1)) = 0
		_Shape_AO_Curvature_Reduction("Shape AO Curvature Reduction", Range( 0 , 1)) = 1
		_ShapeAORemapMin("Base Shape AO Remap Min", Range( 0 , 1)) = 0
		_ShapeAORemapMax("Base Shape AO Remap Max", Range( 0 , 1)) = 1
		_ShapeAORemapMin_1("Cover Shape AO Remap Min", Range( 0 , 1)) = 0
		_ShapeAORemapMax_1("Cover Shape AO Remap Max", Range( 0 , 1)) = 1
		_ShapeHeightMapMin("Shape Height Map Min", Float) = 0
		_ShapeHeightMapMax("Shape Height Map Max", Float) = 1
		_ShapeHeightMapOffset("Shape Height Map Offset", Float) = 0
		_LeaksR("Leaks (R)", 2D) = "white" {}
		[Toggle(LEAKS_UV0_UV2_1_ON)] Leaks_UV0_UV2_1("Leaks UV0 (T) UV2 (F)", Float) = 1
		_BaseLeaksColorMultiply("Base Leaks Color (RGB) Multiply (A)", Color) = (0,0,0,0)
		_CoverLeaksColorMultiply("Cover Leaks Color (RGB) Multiply (A)", Color) = (0,0,0,0)
		_LeaksTilingOffset("Leaks Tiling and Offset", Vector) = (1,1,0,0)
		_LeaksSmoothnessMultiply("Base Leaks Smoothness Multiply", Range( 0 , 2)) = 1
		_LeaksSmoothnessMultiply_1("Cover Leaks Smoothness Multiply", Range( 0 , 2)) = 1
		[Toggle]_Wetness_T_Heat_F("Wetness (T) Heat (F)", Float) = 0
		_WetColor("Wet Color Vertex(R)", Color) = (0,0,0,0)
		_WetSmoothness("Wet Smoothness Vertex(R)", Range( 0 , 1)) = 1
		[HDR]_LavaEmissionColor("Emission Color", Color) = (1,0.1862055,0,0)
		_BaseEmissionMaskIntensivity("Base Emission Mask Intensivity", Range( 0 , 100)) = 0
		_BaseEmissionMaskTreshold("Base Emission Mask Treshold", Range( 0.01 , 100)) = 1
		_CoverEmissionMaskIntensivity("Cover Emission Mask Intensivity", Range( 0 , 100)) = 0
		_CoverEmissionMaskTreshold("Cover Emission Mask Treshold", Range( 0.01 , 100)) = 1
		_BaseEmissionMaskIntensivity_1("Shape Emission Mask Intensivity", Range( 0 , 100)) = 0
		_BaseEmissionMaskTreshold_1("Shape Emission Mask Treshold", Range( 0.01 , 100)) = 1
		[HDR]_RimColor("Rim Color", Color) = (1,0,0,0)
		_RimLightPower("Rim Light Power", Float) = 4
		_Noise("Emission Noise", 2D) = "white" {}
		_NoiseTiling("Emission Noise Tiling", Vector) = (1,1,0,0)
		_NoiseSpeed("Emission Noise Speed", Vector) = (0.001,0.005,0,0)
		_EmissionNoisePower("Emission Noise Power", Range( 0 , 10)) = 2.71
		[Toggle]_Dynamic_Flow("Dynamic Flow", Float) = 0
		_Dynamic_Shape_Speed("Dynamic Shape Speed", Range( 0 , 10)) = 0.1
		_Dynamic_Start_Position_Offset("Dynamic Start Position Offset", Float) = 0
		_Dynamic_Reaction_Offset("Dynamic Reaction Offset", Float) = 0
		_Dynamic_Shape_V_Curve_Power("Dynamic Shape V Curve Power", Range( -8 , 8)) = 1.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _USEDYNAMICCOVERTSTATICMASKF_ON
		#pragma shader_feature_local _USE_SHAPEHEIGHTBT_STATIC_MASKF_ON
		#pragma shader_feature_local _USECOVERHEIGHTT_ON
		#pragma shader_feature_local LEAKS_UV0_UV2_1_ON
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
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
			float3 worldPos;
			float2 uv_texcoord;
			float2 uv3_texcoord3;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_ShapeNormal);
		uniform float4 _ShapeTilingOffset;
		SamplerState sampler_Linear_Repeat;
		uniform float _ShapeNormalStrenght1;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseNormalMap);
		uniform float4 _BaseTilingOffset;
		uniform float _BaseNormalScale;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CoverNormalMap);
		uniform float4 _CoverTilingOffset;
		uniform float _CoverNormalScale;
		uniform float _ShapeNormalStrenght_1;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CoverMaskA);
		SamplerState sampler_CoverMaskA;
		uniform float _CoverMaskPower;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_ShapeCurvAOHLeaksMask);
		uniform float _ShapeHeightMapMin;
		uniform float _ShapeHeightMapOffset;
		uniform float _ShapeHeightMapMax;
		uniform float _CoverHeightMapMax;
		uniform float _CoverHeightMapOffset;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CoverMaskMap);
		uniform float _CoverHeightMapMin;
		uniform float _VertexColorBBlendStrenght;
		uniform float _Shape_Normal_Blend_Hardness;
		uniform float _CoverNormalBlendHardness;
		uniform float _Cover_Amount;
		uniform float _Cover_Amount_Grow_Speed;
		uniform float _Cover_Max_Angle;
		uniform float _CoverHardness;
		uniform float _Cover_Min_Height;
		uniform float _Cover_Min_Height_Blending;
		uniform float _VertexColorGBlendStrenght;
		uniform float4 _BaseColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseColorMap);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_LeaksR);
		uniform float4 _LeaksTilingOffset;
		SamplerState sampler_LeaksR;
		uniform float4 _BaseLeaksColorMultiply;
		uniform float _ShapeAORemapMin;
		uniform float _ShapeAORemapMax;
		uniform float _Shape_AO_Curvature_Reduction;
		uniform float _CurvatureBlend;
		uniform float4 _CoverBaseColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CoverBaseColorMap);
		uniform float4 _CoverLeaksColorMultiply;
		uniform float4 _WetColor;
		uniform float _BaseEmissionMaskIntensivity_1;
		uniform float _BaseEmissionMaskTreshold_1;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BaseMaskMap);
		uniform float _BaseEmissionMaskIntensivity;
		uniform float _BaseEmissionMaskTreshold;
		uniform float _CoverEmissionMaskIntensivity;
		uniform float _CoverEmissionMaskTreshold;
		uniform float4 _LavaEmissionColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Noise);
		uniform float2 _NoiseSpeed;
		uniform float2 _NoiseTiling;
		SamplerState sampler_Noise;
		uniform float _EmissionNoisePower;
		uniform float4 _RimColor;
		uniform float _RimLightPower;
		uniform float _BaseMetallic;
		uniform float _BaseAORemapMin;
		uniform float _BaseAORemapMax;
		uniform float _BaseSmoothnessRemapMin;
		uniform float _BaseSmoothnessRemapMax;
		uniform float _LeaksSmoothnessMultiply;
		uniform float _CoverMetallic;
		uniform float _CoverAORemapMin;
		uniform float _CoverAORemapMax;
		uniform float _ShapeAORemapMin_1;
		uniform float _ShapeAORemapMax_1;
		uniform float _CoverSmoothnessRemapMin;
		uniform float _CoverSmoothnessRemapMax;
		uniform float _LeaksSmoothnessMultiply_1;
		uniform float _WetSmoothness;
		uniform float _Cutoff = 0;

		UNITY_INSTANCING_BUFFER_START(TopCoverShape)
			UNITY_DEFINE_INSTANCED_PROP(float4, _CoverMaskA_ST)
#define _CoverMaskA_ST_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Shape_UsePlanarUV)
#define _Shape_UsePlanarUV_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Shape_UV0_UV2)
#define _Shape_UV0_UV2_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _BaseUsePlanarUV)
#define _BaseUsePlanarUV_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _UV0_UV2)
#define _UV0_UV2_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _CoverUsePlanarUV)
#define _CoverUsePlanarUV_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Cover_UV0_UV2)
#define _Cover_UV0_UV2_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Wetness_T_Heat_F)
#define _Wetness_T_Heat_F_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Dynamic_Flow)
#define _Dynamic_Flow_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Dynamic_Reaction_Offset)
#define _Dynamic_Reaction_Offset_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Dynamic_Start_Position_Offset)
#define _Dynamic_Start_Position_Offset_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Dynamic_Shape_Speed)
#define _Dynamic_Shape_Speed_arr TopCoverShape
			UNITY_DEFINE_INSTANCED_PROP(float, _Dynamic_Shape_V_Curve_Power)
#define _Dynamic_Shape_V_Curve_Power_arr TopCoverShape
		UNITY_INSTANCING_BUFFER_END(TopCoverShape)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float _Shape_UsePlanarUV_Instance = UNITY_ACCESS_INSTANCED_PROP(_Shape_UsePlanarUV_arr, _Shape_UsePlanarUV);
			float3 ase_worldPos = i.worldPos;
			float3 break16_g102 = ase_worldPos;
			float2 appendResult17_g102 = (float2(break16_g102.x , break16_g102.z));
			float4 break29_g102 = _ShapeTilingOffset;
			float _Shape_UV0_UV2_Instance = UNITY_ACCESS_INSTANCED_PROP(_Shape_UV0_UV2_arr, _Shape_UV0_UV2);
			float2 appendResult24_g102 = (float2(break29_g102.x , break29_g102.y));
			float2 appendResult25_g102 = (float2(break29_g102.z , break29_g102.w));
			float2 uv_TexCoord26_g102 = i.uv_texcoord * appendResult24_g102 + appendResult25_g102;
			float2 uv3_TexCoord49_g102 = i.uv3_texcoord3 * appendResult24_g102 + appendResult25_g102;
			float3 tex2DNode13_g102 = UnpackNormal( SAMPLE_TEXTURE2D( _ShapeNormal, sampler_Linear_Repeat, ( _Shape_UsePlanarUV_Instance == 1.0 ? ( appendResult17_g102 * ( 1.0 / break29_g102.x ) ) : ( _Shape_UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g102 : uv3_TexCoord49_g102 ) ) ) );
			float2 appendResult32_g102 = (float2(tex2DNode13_g102.r , tex2DNode13_g102.g));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float2 appendResult39_g102 = (float2(sign( ase_worldNormal ).y , 1.0));
			float3 break41_g102 = ase_worldNormal;
			float2 appendResult42_g102 = (float2(break41_g102.x , break41_g102.z));
			float2 break44_g102 = ( ( appendResult32_g102 * appendResult39_g102 ) + appendResult42_g102 );
			float3 appendResult45_g102 = (float3(break44_g102.x , ( tex2DNode13_g102.b * break41_g102.y ) , break44_g102.y));
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir46_g102 = mul( ase_worldToTangent, appendResult45_g102);
			float3 normalizeResult48_g102 = normalize( worldToTangentDir46_g102 );
			#ifdef _PLANARUV1_ON
				float3 staticSwitch31_g102 = normalizeResult48_g102;
			#else
				float3 staticSwitch31_g102 = tex2DNode13_g102;
			#endif
			float3 temp_output_560_0 = staticSwitch31_g102;
			float3 break31_g114 = temp_output_560_0;
			float2 appendResult35_g114 = (float2(break31_g114.x , break31_g114.y));
			float temp_output_38_0_g114 = _ShapeNormalStrenght1;
			float lerpResult36_g114 = lerp( 1.0 , break31_g114.z , saturate( temp_output_38_0_g114 ));
			float3 appendResult34_g114 = (float3(( appendResult35_g114 * temp_output_38_0_g114 ) , lerpResult36_g114));
			float3 temp_output_313_0 = appendResult34_g114;
			float3 temp_output_39_0_g106 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g106 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g106 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g106 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g106 = ( ( temp_output_39_0_g106 * break29_g106.x ) + ( temp_output_38_0_g106 * break42_g106.x ) );
			float dotResult50_g106 = dot( temp_output_46_0_g106 , temp_output_46_0_g106 );
			float3 temp_output_47_0_g106 = ( ( temp_output_39_0_g106 * break29_g106.y ) + ( temp_output_38_0_g106 * break42_g106.y ) );
			float dotResult49_g106 = dot( temp_output_47_0_g106 , temp_output_47_0_g106 );
			float temp_output_53_0_g106 = ( 1.0 / sqrt( max( dotResult50_g106 , dotResult49_g106 ) ) );
			float3 worldToTangentDir59_g106 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g106 * temp_output_53_0_g106 ).x,( temp_output_47_0_g106 * temp_output_53_0_g106 ).x,ase_worldNormal.x,( temp_output_46_0_g106 * temp_output_53_0_g106 ).y,( temp_output_47_0_g106 * temp_output_53_0_g106 ).y,ase_worldNormal.y,( temp_output_46_0_g106 * temp_output_53_0_g106 ).z,( temp_output_47_0_g106 * temp_output_53_0_g106 ).z,ase_worldNormal.z ), temp_output_313_0 ));
			float3 clampResult61_g106 = clamp( worldToTangentDir59_g106 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float _BaseUsePlanarUV_Instance = UNITY_ACCESS_INSTANCED_PROP(_BaseUsePlanarUV_arr, _BaseUsePlanarUV);
			float3 break16_g108 = ase_worldPos;
			float2 appendResult17_g108 = (float2(break16_g108.x , break16_g108.z));
			float4 break29_g108 = _BaseTilingOffset;
			float _UV0_UV2_Instance = UNITY_ACCESS_INSTANCED_PROP(_UV0_UV2_arr, _UV0_UV2);
			float2 appendResult24_g108 = (float2(break29_g108.x , break29_g108.y));
			float2 appendResult25_g108 = (float2(break29_g108.z , break29_g108.w));
			float2 uv_TexCoord26_g108 = i.uv_texcoord * appendResult24_g108 + appendResult25_g108;
			float2 uv3_TexCoord49_g108 = i.uv3_texcoord3 * appendResult24_g108 + appendResult25_g108;
			float3 tex2DNode13_g108 = UnpackNormal( SAMPLE_TEXTURE2D( _BaseNormalMap, sampler_Linear_Repeat, ( _BaseUsePlanarUV_Instance == 1.0 ? ( appendResult17_g108 * ( 1.0 / break29_g108.x ) ) : ( _UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g108 : uv3_TexCoord49_g108 ) ) ) );
			float2 appendResult32_g108 = (float2(tex2DNode13_g108.r , tex2DNode13_g108.g));
			float2 appendResult39_g108 = (float2(sign( ase_worldNormal ).y , 1.0));
			float3 break41_g108 = ase_worldNormal;
			float2 appendResult42_g108 = (float2(break41_g108.x , break41_g108.z));
			float2 break44_g108 = ( ( appendResult32_g108 * appendResult39_g108 ) + appendResult42_g108 );
			float3 appendResult45_g108 = (float3(break44_g108.x , ( tex2DNode13_g108.b * break41_g108.y ) , break44_g108.y));
			float3 worldToTangentDir46_g108 = mul( ase_worldToTangent, appendResult45_g108);
			float3 normalizeResult48_g108 = normalize( worldToTangentDir46_g108 );
			#ifdef _PLANARUV1_ON
				float3 staticSwitch31_g108 = normalizeResult48_g108;
			#else
				float3 staticSwitch31_g108 = tex2DNode13_g108;
			#endif
			float3 break31_g82 = staticSwitch31_g108;
			float2 appendResult35_g82 = (float2(break31_g82.x , break31_g82.y));
			float temp_output_38_0_g82 = _BaseNormalScale;
			float lerpResult36_g82 = lerp( 1.0 , break31_g82.z , saturate( temp_output_38_0_g82 ));
			float3 appendResult34_g82 = (float3(( appendResult35_g82 * temp_output_38_0_g82 ) , lerpResult36_g82));
			float3 temp_output_69_0 = appendResult34_g82;
			float3 temp_output_39_0_g87 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g87 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g87 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g87 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g87 = ( ( temp_output_39_0_g87 * break29_g87.x ) + ( temp_output_38_0_g87 * break42_g87.x ) );
			float dotResult50_g87 = dot( temp_output_46_0_g87 , temp_output_46_0_g87 );
			float3 temp_output_47_0_g87 = ( ( temp_output_39_0_g87 * break29_g87.y ) + ( temp_output_38_0_g87 * break42_g87.y ) );
			float dotResult49_g87 = dot( temp_output_47_0_g87 , temp_output_47_0_g87 );
			float temp_output_53_0_g87 = ( 1.0 / sqrt( max( dotResult50_g87 , dotResult49_g87 ) ) );
			float3 worldToTangentDir59_g87 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g87 * temp_output_53_0_g87 ).x,( temp_output_47_0_g87 * temp_output_53_0_g87 ).x,ase_worldNormal.x,( temp_output_46_0_g87 * temp_output_53_0_g87 ).y,( temp_output_47_0_g87 * temp_output_53_0_g87 ).y,ase_worldNormal.y,( temp_output_46_0_g87 * temp_output_53_0_g87 ).z,( temp_output_47_0_g87 * temp_output_53_0_g87 ).z,ase_worldNormal.z ), temp_output_69_0 ));
			float3 clampResult61_g87 = clamp( worldToTangentDir59_g87 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float3 temp_output_572_0 = ( _BaseUsePlanarUV_Instance == 1.0 ? temp_output_69_0 : ( _UV0_UV2_Instance == 1.0 ? temp_output_69_0 : clampResult61_g87 ) );
			float _CoverUsePlanarUV_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverUsePlanarUV_arr, _CoverUsePlanarUV);
			float3 break16_g99 = ase_worldPos;
			float2 appendResult17_g99 = (float2(break16_g99.x , break16_g99.z));
			float4 break29_g99 = _CoverTilingOffset;
			float _Cover_UV0_UV2_Instance = UNITY_ACCESS_INSTANCED_PROP(_Cover_UV0_UV2_arr, _Cover_UV0_UV2);
			float2 appendResult24_g99 = (float2(break29_g99.x , break29_g99.y));
			float2 appendResult25_g99 = (float2(break29_g99.z , break29_g99.w));
			float2 uv_TexCoord26_g99 = i.uv_texcoord * appendResult24_g99 + appendResult25_g99;
			float2 uv3_TexCoord50_g99 = i.uv3_texcoord3 * appendResult24_g99 + appendResult25_g99;
			float3 tex2DNode13_g99 = UnpackNormal( SAMPLE_TEXTURE2D( _CoverNormalMap, sampler_Linear_Repeat, ( _CoverUsePlanarUV_Instance == 1.0 ? ( appendResult17_g99 * ( 1.0 / break29_g99.x ) ) : ( _Cover_UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g99 : uv3_TexCoord50_g99 ) ) ) );
			float2 appendResult32_g99 = (float2(tex2DNode13_g99.r , tex2DNode13_g99.g));
			float2 appendResult39_g99 = (float2(sign( ase_worldNormal ).y , 1.0));
			float3 break41_g99 = ase_worldNormal;
			float2 appendResult42_g99 = (float2(break41_g99.x , break41_g99.z));
			float2 break44_g99 = ( ( appendResult32_g99 * appendResult39_g99 ) + appendResult42_g99 );
			float3 appendResult45_g99 = (float3(break44_g99.x , ( tex2DNode13_g99.b * break41_g99.y ) , break44_g99.y));
			float3 worldToTangentDir46_g99 = mul( ase_worldToTangent, appendResult45_g99);
			float3 normalizeResult48_g99 = normalize( worldToTangentDir46_g99 );
			#ifdef _PLANARUV1_ON
				float3 staticSwitch31_g99 = normalizeResult48_g99;
			#else
				float3 staticSwitch31_g99 = tex2DNode13_g99;
			#endif
			float3 temp_output_557_0 = staticSwitch31_g99;
			float3 break31_g65 = temp_output_557_0;
			float2 appendResult35_g65 = (float2(break31_g65.x , break31_g65.y));
			float temp_output_38_0_g65 = _CoverNormalScale;
			float lerpResult36_g65 = lerp( 1.0 , break31_g65.z , saturate( temp_output_38_0_g65 ));
			float3 appendResult34_g65 = (float3(( appendResult35_g65 * temp_output_38_0_g65 ) , lerpResult36_g65));
			float3 temp_output_169_0 = appendResult34_g65;
			float3 temp_output_39_0_g89 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g89 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g89 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g89 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g89 = ( ( temp_output_39_0_g89 * break29_g89.x ) + ( temp_output_38_0_g89 * break42_g89.x ) );
			float dotResult50_g89 = dot( temp_output_46_0_g89 , temp_output_46_0_g89 );
			float3 temp_output_47_0_g89 = ( ( temp_output_39_0_g89 * break29_g89.y ) + ( temp_output_38_0_g89 * break42_g89.y ) );
			float dotResult49_g89 = dot( temp_output_47_0_g89 , temp_output_47_0_g89 );
			float temp_output_53_0_g89 = ( 1.0 / sqrt( max( dotResult50_g89 , dotResult49_g89 ) ) );
			float3 worldToTangentDir59_g89 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g89 * temp_output_53_0_g89 ).x,( temp_output_47_0_g89 * temp_output_53_0_g89 ).x,ase_worldNormal.x,( temp_output_46_0_g89 * temp_output_53_0_g89 ).y,( temp_output_47_0_g89 * temp_output_53_0_g89 ).y,ase_worldNormal.y,( temp_output_46_0_g89 * temp_output_53_0_g89 ).z,( temp_output_47_0_g89 * temp_output_53_0_g89 ).z,ase_worldNormal.z ), temp_output_169_0 ));
			float3 clampResult61_g89 = clamp( worldToTangentDir59_g89 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float3 break31_g76 = temp_output_560_0;
			float2 appendResult35_g76 = (float2(break31_g76.x , break31_g76.y));
			float temp_output_38_0_g76 = _ShapeNormalStrenght_1;
			float lerpResult36_g76 = lerp( 1.0 , break31_g76.z , saturate( temp_output_38_0_g76 ));
			float3 appendResult34_g76 = (float3(( appendResult35_g76 * temp_output_38_0_g76 ) , lerpResult36_g76));
			float3 temp_output_314_0 = appendResult34_g76;
			float3 temp_output_39_0_g111 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g111 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g111 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g111 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g111 = ( ( temp_output_39_0_g111 * break29_g111.x ) + ( temp_output_38_0_g111 * break42_g111.x ) );
			float dotResult50_g111 = dot( temp_output_46_0_g111 , temp_output_46_0_g111 );
			float3 temp_output_47_0_g111 = ( ( temp_output_39_0_g111 * break29_g111.y ) + ( temp_output_38_0_g111 * break42_g111.y ) );
			float dotResult49_g111 = dot( temp_output_47_0_g111 , temp_output_47_0_g111 );
			float temp_output_53_0_g111 = ( 1.0 / sqrt( max( dotResult50_g111 , dotResult49_g111 ) ) );
			float3 worldToTangentDir59_g111 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g111 * temp_output_53_0_g111 ).x,( temp_output_47_0_g111 * temp_output_53_0_g111 ).x,ase_worldNormal.x,( temp_output_46_0_g111 * temp_output_53_0_g111 ).y,( temp_output_47_0_g111 * temp_output_53_0_g111 ).y,ase_worldNormal.y,( temp_output_46_0_g111 * temp_output_53_0_g111 ).z,( temp_output_47_0_g111 * temp_output_53_0_g111 ).z,ase_worldNormal.z ), temp_output_314_0 ));
			float3 clampResult61_g111 = clamp( worldToTangentDir59_g111 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float3 temp_output_317_0 = BlendNormals( ( _CoverUsePlanarUV_Instance == 1.0 ? temp_output_169_0 : ( _Cover_UV0_UV2_Instance == 1.0 ? temp_output_169_0 : clampResult61_g89 ) ) , ( _Shape_UsePlanarUV_Instance == 1.0 ? temp_output_314_0 : ( _Shape_UV0_UV2_Instance == 1.0 ? temp_output_314_0 : clampResult61_g111 ) ) );
			float4 _CoverMaskA_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverMaskA_ST_arr, _CoverMaskA_ST);
			float2 uv_CoverMaskA = i.uv_texcoord * _CoverMaskA_ST_Instance.xy + _CoverMaskA_ST_Instance.zw;
			float temp_output_241_0 = ( SAMPLE_TEXTURE2D( _CoverMaskA, sampler_CoverMaskA, uv_CoverMaskA ).a * _CoverMaskPower );
			float3 break16_g115 = ase_worldPos;
			float2 appendResult17_g115 = (float2(break16_g115.x , break16_g115.z));
			float4 break29_g115 = _ShapeTilingOffset;
			float2 appendResult24_g115 = (float2(break29_g115.x , break29_g115.y));
			float2 appendResult25_g115 = (float2(break29_g115.z , break29_g115.w));
			float2 uv_TexCoord26_g115 = i.uv_texcoord * appendResult24_g115 + appendResult25_g115;
			float2 uv3_TexCoord31_g115 = i.uv3_texcoord3 * appendResult24_g115 + appendResult25_g115;
			float4 break508 = SAMPLE_TEXTURE2D( _ShapeCurvAOHLeaksMask, sampler_Linear_Repeat, ( _Shape_UsePlanarUV_Instance == 1.0 ? ( appendResult17_g115 * ( 1.0 / break29_g115.x ) ) : ( _Shape_UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g115 : uv3_TexCoord31_g115 ) ) );
			float temp_output_343_0 = ( _ShapeHeightMapMin + _ShapeHeightMapOffset );
			float temp_output_344_0 = ( _ShapeHeightMapMax + _ShapeHeightMapOffset );
			float temp_output_342_0 = (temp_output_343_0 + (break508.b - 0.0) * (temp_output_344_0 - temp_output_343_0) / (1.0 - 0.0));
			#ifdef _USE_SHAPEHEIGHTBT_STATIC_MASKF_ON
				float staticSwitch401 = temp_output_342_0;
			#else
				float staticSwitch401 = temp_output_241_0;
			#endif
			float clampResult243 = clamp( staticSwitch401 , 0.0 , 1.0 );
			float temp_output_233_0 = ( _CoverHeightMapMax + _CoverHeightMapOffset );
			float3 break16_g97 = ase_worldPos;
			float2 appendResult17_g97 = (float2(break16_g97.x , break16_g97.z));
			float4 break29_g97 = _CoverTilingOffset;
			float2 appendResult24_g97 = (float2(break29_g97.x , break29_g97.y));
			float2 appendResult25_g97 = (float2(break29_g97.z , break29_g97.w));
			float2 uv_TexCoord26_g97 = i.uv_texcoord * appendResult24_g97 + appendResult25_g97;
			float2 uv3_TexCoord32_g97 = i.uv3_texcoord3 * appendResult24_g97 + appendResult25_g97;
			float4 break158 = SAMPLE_TEXTURE2D( _CoverMaskMap, sampler_Linear_Repeat, ( _CoverUsePlanarUV_Instance == 1.0 ? ( appendResult17_g97 * ( 1.0 / break29_g97.x ) ) : ( _Cover_UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g97 : uv3_TexCoord32_g97 ) ) );
			float temp_output_232_0 = ( _CoverHeightMapMin + _CoverHeightMapOffset );
			#ifdef _USECOVERHEIGHTT_ON
				float staticSwitch407 = (temp_output_232_0 + (break158.b - 0.0) * (temp_output_233_0 - temp_output_232_0) / (1.0 - 0.0));
			#else
				float staticSwitch407 = temp_output_233_0;
			#endif
			float clampResult501 = clamp( i.vertexColor.b , 0.0 , 1.0 );
			float2 appendResult431 = (float2(temp_output_232_0 , temp_output_233_0));
			float2 break430 = appendResult431;
			float2 break437 = ( appendResult431 + abs( ( clampResult501 * ( break430.y - break430.x ) ) ) );
			#ifdef _USECOVERHEIGHTT_ON
				float staticSwitch447 = (break437.x + (break158.b - 0.0) * (break437.y - break437.x) / (1.0 - 0.0));
			#else
				float staticSwitch447 = clampResult501;
			#endif
			float clampResult439 = clamp( staticSwitch447 , 0.0 , 1.0 );
			float2 appendResult422 = (float2(temp_output_343_0 , temp_output_344_0));
			float2 break420 = appendResult422;
			float2 break426 = ( appendResult422 + abs( ( ( break420.y - break420.x ) * clampResult501 ) ) );
			float clampResult427 = clamp( (break426.x + (break508.b - 0.0) * (break426.y - break426.x) / (1.0 - 0.0)) , 0.0 , 1.0 );
			#ifdef _USE_SHAPEHEIGHTBT_STATIC_MASKF_ON
				float staticSwitch440 = ( clampResult427 * clampResult439 );
			#else
				float staticSwitch440 = clampResult439;
			#endif
			float clampResult446 = clamp( ( ( 1.0 - clampResult501 ) > 0.99 ? 0.0 : pow( abs( saturate( staticSwitch440 ) ) , _VertexColorBBlendStrenght ) ) , 0.0 , 1.0 );
			float3 break31_g104 = temp_output_560_0;
			float2 appendResult35_g104 = (float2(break31_g104.x , break31_g104.y));
			float temp_output_38_0_g104 = _Shape_Normal_Blend_Hardness;
			float lerpResult36_g104 = lerp( 1.0 , break31_g104.z , saturate( temp_output_38_0_g104 ));
			float3 appendResult34_g104 = (float3(( appendResult35_g104 * temp_output_38_0_g104 ) , lerpResult36_g104));
			float3 temp_output_509_0 = appendResult34_g104;
			float3 temp_output_39_0_g112 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g112 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g112 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g112 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g112 = ( ( temp_output_39_0_g112 * break29_g112.x ) + ( temp_output_38_0_g112 * break42_g112.x ) );
			float dotResult50_g112 = dot( temp_output_46_0_g112 , temp_output_46_0_g112 );
			float3 temp_output_47_0_g112 = ( ( temp_output_39_0_g112 * break29_g112.y ) + ( temp_output_38_0_g112 * break42_g112.y ) );
			float dotResult49_g112 = dot( temp_output_47_0_g112 , temp_output_47_0_g112 );
			float temp_output_53_0_g112 = ( 1.0 / sqrt( max( dotResult50_g112 , dotResult49_g112 ) ) );
			float3 worldToTangentDir59_g112 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g112 * temp_output_53_0_g112 ).x,( temp_output_47_0_g112 * temp_output_53_0_g112 ).x,ase_worldNormal.x,( temp_output_46_0_g112 * temp_output_53_0_g112 ).y,( temp_output_47_0_g112 * temp_output_53_0_g112 ).y,ase_worldNormal.y,( temp_output_46_0_g112 * temp_output_53_0_g112 ).z,( temp_output_47_0_g112 * temp_output_53_0_g112 ).z,ase_worldNormal.z ), temp_output_509_0 ));
			float3 clampResult61_g112 = clamp( worldToTangentDir59_g112 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float3 break31_g86 = temp_output_557_0;
			float2 appendResult35_g86 = (float2(break31_g86.x , break31_g86.y));
			float temp_output_38_0_g86 = _CoverNormalBlendHardness;
			float lerpResult36_g86 = lerp( 1.0 , break31_g86.z , saturate( temp_output_38_0_g86 ));
			float3 appendResult34_g86 = (float3(( appendResult35_g86 * temp_output_38_0_g86 ) , lerpResult36_g86));
			float3 temp_output_181_0 = appendResult34_g86;
			float3 temp_output_39_0_g110 = cross( ddx( ase_worldPos ) , ase_worldNormal );
			float2 break29_g110 = ddy( i.uv3_texcoord3 );
			float3 temp_output_38_0_g110 = cross( ase_worldNormal , ddy( ase_worldPos ) );
			float2 break42_g110 = ddx( i.uv3_texcoord3 );
			float3 temp_output_46_0_g110 = ( ( temp_output_39_0_g110 * break29_g110.x ) + ( temp_output_38_0_g110 * break42_g110.x ) );
			float dotResult50_g110 = dot( temp_output_46_0_g110 , temp_output_46_0_g110 );
			float3 temp_output_47_0_g110 = ( ( temp_output_39_0_g110 * break29_g110.y ) + ( temp_output_38_0_g110 * break42_g110.y ) );
			float dotResult49_g110 = dot( temp_output_47_0_g110 , temp_output_47_0_g110 );
			float temp_output_53_0_g110 = ( 1.0 / sqrt( max( dotResult50_g110 , dotResult49_g110 ) ) );
			float3 worldToTangentDir59_g110 = mul( ase_worldToTangent, mul( float3x3(( temp_output_46_0_g110 * temp_output_53_0_g110 ).x,( temp_output_47_0_g110 * temp_output_53_0_g110 ).x,ase_worldNormal.x,( temp_output_46_0_g110 * temp_output_53_0_g110 ).y,( temp_output_47_0_g110 * temp_output_53_0_g110 ).y,ase_worldNormal.y,( temp_output_46_0_g110 * temp_output_53_0_g110 ).z,( temp_output_47_0_g110 * temp_output_53_0_g110 ).z,ase_worldNormal.z ), temp_output_181_0 ));
			float3 clampResult61_g110 = clamp( worldToTangentDir59_g110 , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float temp_output_187_0 = ( 4.0 - _Cover_Amount_Grow_Speed );
			float clampResult190 = clamp( pow( abs( ( _Cover_Amount / temp_output_187_0 ) ) , temp_output_187_0 ) , 0.0 , 2.0 );
			float clampResult192 = clamp( ase_worldNormal.y , 0.0 , 0.9999 );
			float temp_output_195_0 = ( _Cover_Max_Angle / 45.0 );
			float clampResult198 = clamp( ( clampResult192 - ( 1.0 - temp_output_195_0 ) ) , 0.0 , 2.0 );
			float temp_output_209_0 = ( ( 1.0 - _Cover_Min_Height ) + ase_worldPos.y );
			float clampResult217 = clamp( ( temp_output_209_0 + 1.0 ) , 0.0 , 1.0 );
			float clampResult219 = clamp( ( ( 1.0 - ( ( temp_output_209_0 + _Cover_Min_Height_Blending ) / temp_output_209_0 ) ) + -0.5 ) , 0.0 , 1.0 );
			float clampResult221 = clamp( ( clampResult217 + clampResult219 ) , 0.0 , 1.0 );
			float temp_output_206_0 = ( pow( abs( ( clampResult198 * ( 1.0 / temp_output_195_0 ) ) ) , _CoverHardness ) * clampResult221 );
			float3 lerpResult223 = lerp( BlendNormals( ( _Shape_UsePlanarUV_Instance == 1.0 ? temp_output_509_0 : ( _Shape_UV0_UV2_Instance == 1.0 ? temp_output_509_0 : clampResult61_g112 ) ) , temp_output_572_0 ) , ( _CoverUsePlanarUV_Instance == 1.0 ? temp_output_181_0 : ( _Cover_UV0_UV2_Instance == 1.0 ? temp_output_181_0 : clampResult61_g110 ) ) , ( saturate( ( clampResult190 * ase_worldNormal.y ) ) * temp_output_206_0 ));
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir224 = mul( ase_tangentToWorldFast, lerpResult223 );
			float temp_output_226_0 = ( ( tangentToWorldDir224.y * clampResult190 ) * ( ( _CoverHardness * clampResult190 ) * temp_output_206_0 ) );
			float clampResult405 = clamp( ( saturate( temp_output_226_0 ) * temp_output_342_0 ) , 0.0 , 1.0 );
			#ifdef _USE_SHAPEHEIGHTBT_STATIC_MASKF_ON
				float staticSwitch402 = clampResult405;
			#else
				float staticSwitch402 = temp_output_241_0;
			#endif
			float clampResult412 = clamp( staticSwitch402 , 0.0 , 1.0 );
			float clampResult244 = clamp( ( clampResult412 * saturate( ( ( temp_output_226_0 * staticSwitch407 ) * clampResult446 ) ) ) , 0.0 , 1.0 );
			#ifdef _USEDYNAMICCOVERTSTATICMASKF_ON
				float staticSwitch245 = clampResult244;
			#else
				float staticSwitch245 = ( clampResult243 * saturate( ( staticSwitch407 * clampResult446 ) ) );
			#endif
			float3 lerpResult247 = lerp( BlendNormals( ( _Shape_UsePlanarUV_Instance == 1.0 ? temp_output_313_0 : ( _Shape_UV0_UV2_Instance == 1.0 ? temp_output_313_0 : clampResult61_g106 ) ) , temp_output_572_0 ) , temp_output_317_0 , staticSwitch245);
			float clampResult502 = clamp( i.vertexColor.g , 0.0 , 1.0 );
			float2 appendResult462 = (float2(temp_output_232_0 , temp_output_233_0));
			float clampResult503 = clamp( i.vertexColor.g , 0.0 , 1.0 );
			float clampResult477 = clamp( ( 1.0 - clampResult503 ) , 0.0 , 1.0 );
			float2 break460 = appendResult462;
			float2 break476 = ( appendResult462 + abs( ( clampResult477 * ( break460.y - break460.x ) ) ) );
			#ifdef _USECOVERHEIGHTT_ON
				float staticSwitch474 = (break476.x + (break158.b - 0.0) * (break476.y - break476.x) / (1.0 - 0.0));
			#else
				float staticSwitch474 = 1.0;
			#endif
			float clampResult473 = clamp( staticSwitch474 , 0.0 , 1.0 );
			float2 appendResult455 = (float2(temp_output_343_0 , temp_output_344_0));
			float2 break454 = appendResult455;
			float2 break465 = ( appendResult455 + abs( ( ( break454.y - break454.x ) * clampResult477 ) ) );
			float clampResult468 = clamp( (break465.x + (break508.b - 0.0) * (break465.y - break465.x) / (1.0 - 0.0)) , 0.0 , 1.0 );
			#ifdef _USE_SHAPEHEIGHTBT_STATIC_MASKF_ON
				float staticSwitch483 = ( clampResult468 * clampResult473 );
			#else
				float staticSwitch483 = clampResult473;
			#endif
			float clampResult472 = clamp( ( clampResult502 > 0.99 ? 0.0 : pow( abs( saturate( staticSwitch483 ) ) , _VertexColorGBlendStrenght ) ) , 0.0 , 1.0 );
			float HeightLerp491 = clampResult472;
			float3 lerpResult489 = lerp( lerpResult247 , temp_output_317_0 , HeightLerp491);
			o.Normal = lerpResult489;
			float _Wetness_T_Heat_F_Instance = UNITY_ACCESS_INSTANCED_PROP(_Wetness_T_Heat_F_arr, _Wetness_T_Heat_F);
			float3 appendResult64 = (float3(_BaseColor.r , _BaseColor.g , _BaseColor.b));
			float3 break16_g107 = ase_worldPos;
			float2 appendResult17_g107 = (float2(break16_g107.x , break16_g107.z));
			float4 break29_g107 = _BaseTilingOffset;
			float2 appendResult24_g107 = (float2(break29_g107.x , break29_g107.y));
			float2 appendResult25_g107 = (float2(break29_g107.z , break29_g107.w));
			float2 uv_TexCoord26_g107 = i.uv_texcoord * appendResult24_g107 + appendResult25_g107;
			float2 uv3_TexCoord31_g107 = i.uv3_texcoord3 * appendResult24_g107 + appendResult25_g107;
			float4 break60 = SAMPLE_TEXTURE2D( _BaseColorMap, sampler_Linear_Repeat, ( _BaseUsePlanarUV_Instance == 1.0 ? ( appendResult17_g107 * ( 1.0 / break29_g107.x ) ) : ( _UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g107 : uv3_TexCoord31_g107 ) ) );
			float3 appendResult61 = (float3(break60.r , break60.g , break60.b));
			float3 temp_output_63_0 = ( appendResult64 * appendResult61 );
			float2 appendResult354 = (float2(_LeaksTilingOffset.x , _LeaksTilingOffset.y));
			float2 appendResult355 = (float2(_LeaksTilingOffset.z , _LeaksTilingOffset.w));
			float2 uv3_TexCoord358 = i.uv3_texcoord3 * appendResult354 + appendResult355;
			float2 uv_TexCoord357 = i.uv_texcoord * appendResult354 + appendResult355;
			#ifdef LEAKS_UV0_UV2_1_ON
				float2 staticSwitch359 = uv_TexCoord357;
			#else
				float2 staticSwitch359 = uv3_TexCoord358;
			#endif
			float LeaksRvalue362 = SAMPLE_TEXTURE2D( _LeaksR, sampler_LeaksR, staticSwitch359 ).r;
			float4 lerpResult364 = lerp( ( ( 1.0 - LeaksRvalue362 ) * _BaseLeaksColorMultiply ) , float4( 1,1,1,1 ) , LeaksRvalue362);
			float clampResult373 = clamp( ( saturate( _BaseLeaksColorMultiply.a ) * ( 1.0 - break508.a ) ) , 0.0 , 1.0 );
			float4 lerpResult374 = lerp( float4( temp_output_63_0 , 0.0 ) , ( lerpResult364 * float4( temp_output_63_0 , 0.0 ) ) , clampResult373);
			float4 temp_cast_2 = (( ( 1.0 - break508.r ) - 0.5 )).xxxx;
			float temp_output_322_0 = ( break508.r - 0.5 );
			float4 appendResult588 = (float4(temp_output_322_0 , temp_output_322_0 , temp_output_322_0 , temp_output_322_0));
			float4 temp_output_324_0 = saturate( ( ( lerpResult374 - temp_cast_2 ) + appendResult588 ) );
			float temp_output_326_0 = (_ShapeAORemapMin + (break508.g - 0.0) * (_ShapeAORemapMax - _ShapeAORemapMin) / (1.0 - 0.0));
			float4 appendResult586 = (float4(temp_output_326_0 , temp_output_326_0 , temp_output_326_0 , temp_output_326_0));
			float4 lerpResult590 = lerp( temp_output_324_0 , ( temp_output_324_0 * saturate( appendResult586 ) ) , _Shape_AO_Curvature_Reduction);
			float4 lerpResult340 = lerp( lerpResult374 , lerpResult590 , _CurvatureBlend);
			float3 appendResult114 = (float3(lerpResult340.rgb));
			float3 appendResult168 = (float3(_CoverBaseColor.r , _CoverBaseColor.g , _CoverBaseColor.b));
			float3 break16_g98 = ase_worldPos;
			float2 appendResult17_g98 = (float2(break16_g98.x , break16_g98.z));
			float4 break29_g98 = _CoverTilingOffset;
			float2 appendResult24_g98 = (float2(break29_g98.x , break29_g98.y));
			float2 appendResult25_g98 = (float2(break29_g98.z , break29_g98.w));
			float2 uv_TexCoord26_g98 = i.uv_texcoord * appendResult24_g98 + appendResult25_g98;
			float2 uv3_TexCoord32_g98 = i.uv3_texcoord3 * appendResult24_g98 + appendResult25_g98;
			float4 break157 = SAMPLE_TEXTURE2D( _CoverBaseColorMap, sampler_Linear_Repeat, ( _CoverUsePlanarUV_Instance == 1.0 ? ( appendResult17_g98 * ( 1.0 / break29_g98.x ) ) : ( _Cover_UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g98 : uv3_TexCoord32_g98 ) ) );
			float3 appendResult153 = (float3(break157.r , break157.g , break157.b));
			float3 temp_output_159_0 = ( appendResult168 * appendResult153 );
			float4 lerpResult387 = lerp( ( ( 1.0 - LeaksRvalue362 ) * _CoverLeaksColorMultiply ) , float4( 1,1,1,1 ) , LeaksRvalue362);
			float clampResult390 = clamp( ( saturate( _CoverLeaksColorMultiply.a ) * 0.0 ) , 0.0 , 1.0 );
			float4 lerpResult391 = lerp( float4( temp_output_159_0 , 0.0 ) , ( lerpResult387 * float4( temp_output_159_0 , 0.0 ) ) , clampResult390);
			float3 appendResult175 = (float3(lerpResult391.rgb));
			float3 lerpResult246 = lerp( appendResult114 , appendResult175 , staticSwitch245);
			float3 lerpResult488 = lerp( lerpResult246 , appendResult175 , HeightLerp491);
			float3 appendResult294 = (float3(_WetColor.r , _WetColor.g , _WetColor.b));
			float3 appendResult296 = (float3(lerpResult488));
			float _Dynamic_Flow_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dynamic_Flow_arr, _Dynamic_Flow);
			float clampResult132 = clamp( i.vertexColor.r , 0.0 , 1.0 );
			float temp_output_133_0 = ( 1.0 - clampResult132 );
			float _Dynamic_Reaction_Offset_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dynamic_Reaction_Offset_arr, _Dynamic_Reaction_Offset);
			float _Dynamic_Start_Position_Offset_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dynamic_Start_Position_Offset_arr, _Dynamic_Start_Position_Offset);
			float _Dynamic_Shape_Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dynamic_Shape_Speed_arr, _Dynamic_Shape_Speed);
			float temp_output_122_0 = ( _Dynamic_Reaction_Offset_Instance + ( _Dynamic_Start_Position_Offset_Instance + ( _Time.y * _Dynamic_Shape_Speed_Instance ) ) );
			float _Dynamic_Shape_V_Curve_Power_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dynamic_Shape_V_Curve_Power_arr, _Dynamic_Shape_V_Curve_Power);
			float smoothstepResult125 = smoothstep( temp_output_122_0 , ( temp_output_122_0 + _Dynamic_Shape_V_Curve_Power_Instance ) , i.uv3_texcoord3.x);
			float clampResult129 = clamp( ( 1.0 - smoothstepResult125 ) , 0.0 , 1.0 );
			float temp_output_504_0 = ( _Dynamic_Flow_Instance == 1.0 ? ( temp_output_133_0 * clampResult129 ) : temp_output_133_0 );
			float3 lerpResult300 = lerp( lerpResult488 , ( appendResult294 * appendResult296 ) , temp_output_504_0);
			o.Albedo = ( _Wetness_T_Heat_F_Instance == 1.0 ? lerpResult300 : lerpResult488 );
			float4 color584 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float lerpResult543 = lerp( 0.0 , ( 1.0 - break508.g ) , temp_output_504_0);
			float3 break16_g109 = ase_worldPos;
			float2 appendResult17_g109 = (float2(break16_g109.x , break16_g109.z));
			float4 break29_g109 = _BaseTilingOffset;
			float2 appendResult24_g109 = (float2(break29_g109.x , break29_g109.y));
			float2 appendResult25_g109 = (float2(break29_g109.z , break29_g109.w));
			float2 uv_TexCoord26_g109 = i.uv_texcoord * appendResult24_g109 + appendResult25_g109;
			float2 uv3_TexCoord31_g109 = i.uv3_texcoord3 * appendResult24_g109 + appendResult25_g109;
			float4 break72 = SAMPLE_TEXTURE2D( _BaseMaskMap, sampler_Linear_Repeat, ( _BaseUsePlanarUV_Instance == 1.0 ? ( appendResult17_g109 * ( 1.0 / break29_g109.x ) ) : ( _UV0_UV2_Instance == 1.0 ? uv_TexCoord26_g109 : uv3_TexCoord31_g109 ) ) );
			float lerpResult135 = lerp( 0.0 , ( 1.0 - break72.g ) , temp_output_504_0);
			float lerpResult136 = lerp( 0.0 , break158.g , temp_output_504_0);
			float lerpResult142 = lerp( pow( abs( ( lerpResult135 * _BaseEmissionMaskIntensivity ) ) , _BaseEmissionMaskTreshold ) , pow( abs( ( lerpResult136 * _CoverEmissionMaskIntensivity ) ) , _CoverEmissionMaskTreshold ) , staticSwitch245);
			float temp_output_549_0 = max( pow( abs( ( lerpResult543 * _BaseEmissionMaskIntensivity_1 ) ) , _BaseEmissionMaskTreshold_1 ) , lerpResult142 );
			float3 appendResult287 = (float3(_LavaEmissionColor.r , _LavaEmissionColor.g , _LavaEmissionColor.b));
			float2 uv_TexCoord259 = i.uv_texcoord * _NoiseTiling;
			float2 temp_output_258_0 = ( ( _NoiseSpeed * _Time.y ) + uv_TexCoord259 );
			float clampResult271 = clamp( ( pow( abs( min( SAMPLE_TEXTURE2D( _Noise, sampler_Noise, temp_output_258_0 ).a , SAMPLE_TEXTURE2D( _Noise, sampler_Noise, ( ( temp_output_258_0 * float2( -1.2,-0.9 ) ) + float2( 0.5,0.5 ) ) ).a ) ) , _EmissionNoisePower ) * 20.0 ) , 0.05 , 1.2 );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_tanViewDir = mul( ase_worldToTangent, ase_worldViewDir );
			float dotResult273 = dot( lerpResult489 , ase_tanViewDir );
			float3 appendResult281 = (float3(_RimColor.r , _RimColor.g , _RimColor.b));
			float3 temp_output_284_0 = ( ( ( temp_output_549_0 * appendResult287 ) * clampResult271 ) + ( ( ( pow( abs( ( 1.0 - saturate( dotResult273 ) ) ) , 10.0 ) * appendResult281 ) * _RimLightPower ) * temp_output_549_0 ) );
			float3 clampResult290 = clamp( temp_output_284_0 , float3( 0,0,0 ) , temp_output_284_0 );
			o.Emission = ( _Wetness_T_Heat_F_Instance == 1.0 ? color584 : float4( clampResult290 , 0.0 ) ).rgb;
			float temp_output_65_0 = (_BaseSmoothnessRemapMin + (break72.a - 0.0) * (_BaseSmoothnessRemapMax - _BaseSmoothnessRemapMin) / (1.0 - 0.0));
			float lerpResult377 = lerp( ( temp_output_65_0 * _LeaksSmoothnessMultiply ) , temp_output_65_0 , LeaksRvalue362);
			float lerpResult380 = lerp( temp_output_65_0 , lerpResult377 , clampResult373);
			float clampResult381 = clamp( lerpResult380 , 0.0 , 1.0 );
			float3 appendResult79 = (float3(( break72.r * _BaseMetallic ) , ( (_BaseAORemapMin + (break72.g - 0.0) * (_BaseAORemapMax - _BaseAORemapMin) / (1.0 - 0.0)) * temp_output_326_0 ) , clampResult381));
			float temp_output_154_0 = (_CoverSmoothnessRemapMin + (break158.a - 0.0) * (_CoverSmoothnessRemapMax - _CoverSmoothnessRemapMin) / (1.0 - 0.0));
			float lerpResult398 = lerp( ( temp_output_154_0 * _LeaksSmoothnessMultiply_1 ) , temp_output_154_0 , LeaksRvalue362);
			float lerpResult399 = lerp( temp_output_154_0 , lerpResult398 , clampResult390);
			float clampResult394 = clamp( lerpResult399 , 0.0 , 1.0 );
			float3 appendResult172 = (float3(( break158.r * _CoverMetallic ) , ( (_CoverAORemapMin + (break158.g - 0.0) * (_CoverAORemapMax - _CoverAORemapMin) / (1.0 - 0.0)) * (_ShapeAORemapMin_1 + (break508.g - 0.0) * (_ShapeAORemapMax_1 - _ShapeAORemapMin_1) / (1.0 - 0.0)) ) , clampResult394));
			float3 lerpResult248 = lerp( appendResult79 , appendResult172 , staticSwitch245);
			float3 lerpResult490 = lerp( lerpResult248 , appendResult172 , HeightLerp491);
			float3 break252 = lerpResult490;
			o.Metallic = break252;
			float lerpResult536 = lerp( break252.z , _WetSmoothness , temp_output_504_0);
			o.Smoothness = ( _Wetness_T_Heat_F_Instance == 1.0 ? lerpResult536 : break252.z );
			o.Occlusion = break252.y;
			o.Alpha = 1;
			float temp_output_508_3 = break508.a;
			clip( temp_output_508_3 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade 

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
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv3_texcoord3;
				o.customPack1.zw = v.texcoord2;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv3_texcoord3 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
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