Shader "NatureManufacture/Particles/Lava Emissive"
{
	Properties
	{
		_AlphaClipThreshold("Alpha Clip Threshold", Range( 0 , 1)) = 1
		_OpacityMultiply("Opacity Multiply", Float) = 1
		[Toggle(_ReadAlbedo)] _ReadAlbedo("Read Albedo", Float) = 1
		_ParticleMask("Particle (RGB) Mask (A)", 2D) = "white" {}
		_TilingandOffset("Tiling and Offset", Vector) = (0,0,0,0)
		_ParticleColor("Particle Color (RGB) Alpha (A)", Color) = (1,1,1,1)
		_ParticleNormal("Particle Normal", 2D) = "bump" {}
		_ParticleNormalScale("Particle Normal Strenght", Float) = 1
		_Mask_Map("Mask Map (MT_AO_H_SM)", 2D) = "white" {}
		_Smoothness_multiplier("Smoothness multiplier", Range( 0 , 1)) = 1
		_AO_multiplier("AO multiplier", Range( 0 , 1)) = 1
		Vector1_bab3a2e609c74b6baff7b028a65ce418("Metallic multiplier", Range( 0 , 1)) = 1
		_Emission_Texture("Emission Texture", 2D) = "white" {}
		[HDR]_Emission_Color("Emission Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ReadAlbedo
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#endif//ASE Sampling Macros

		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_ParticleNormal);
		uniform float4 _TilingandOffset;
		SamplerState sampler_ParticleNormal;
		uniform float _ParticleNormalScale;
		uniform float4 _ParticleColor;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_ParticleMask);
		SamplerState sampler_ParticleMask;
		uniform float _OpacityMultiply;
		uniform float _AlphaClipThreshold;
		uniform float4 _Emission_Color;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission_Texture);
		SamplerState sampler_Emission_Texture;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Mask_Map);
		SamplerState sampler_Mask_Map;
		uniform float Vector1_bab3a2e609c74b6baff7b028a65ce418;
		uniform float _Smoothness_multiplier;
		uniform float _AO_multiplier;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult278 = (float2(_TilingandOffset.x , _TilingandOffset.y));
			float2 appendResult279 = (float2(_TilingandOffset.z , _TilingandOffset.w));
			float2 uv_TexCoord276 = i.uv_texcoord * appendResult278 + appendResult279;
			float3 break31_g4 = UnpackNormal( SAMPLE_TEXTURE2D( _ParticleNormal, sampler_ParticleNormal, uv_TexCoord276 ) );
			float2 appendResult35_g4 = (float2(break31_g4.x , break31_g4.y));
			float temp_output_38_0_g4 = _ParticleNormalScale;
			float lerpResult36_g4 = lerp( 1.0 , break31_g4.z , saturate( temp_output_38_0_g4 ));
			float3 appendResult34_g4 = (float3(( appendResult35_g4 * temp_output_38_0_g4 ) , lerpResult36_g4));
			o.Normal = appendResult34_g4;
			float4 tex2DNode280 = SAMPLE_TEXTURE2D( _ParticleMask, sampler_ParticleMask, uv_TexCoord276 );
			#ifdef _ReadAlbedo
				float4 staticSwitch272 = ( _ParticleColor * tex2DNode280 );
			#else
				float4 staticSwitch272 = _ParticleColor;
			#endif
			float4 clampResult311 = clamp( i.vertexColor , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float3 appendResult313 = (float3(clampResult311.rgb));
			float clampResult321 = clamp( ( _OpacityMultiply * ( ( _ParticleColor.a * tex2DNode280.a ) * clampResult311.a ) ) , 0.0 , 1.0 );
			clip( clampResult321 - _AlphaClipThreshold);
			o.Albedo = ( staticSwitch272 * float4( appendResult313 , 0.0 ) ).rgb;
			o.Emission = ( ( _Emission_Color * SAMPLE_TEXTURE2D( _Emission_Texture, sampler_Emission_Texture, uv_TexCoord276 ) ) * float4( appendResult313 , 0.0 ) ).rgb;
			float4 break301 = SAMPLE_TEXTURE2D( _Mask_Map, sampler_Mask_Map, uv_TexCoord276 );
			o.Metallic = ( break301.r * Vector1_bab3a2e609c74b6baff7b028a65ce418 );
			o.Smoothness = ( break301.a * _Smoothness_multiplier );
			o.Occlusion = ( break301.g * _AO_multiplier );
			o.Alpha = clampResult321;
			clip( tex2DNode280.a - _AlphaClipThreshold );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
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
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}