// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ProgressBar_BorderNoise"
{
	Properties
	{
		[HideInInspector]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		[HideInInspector]_MainTex ("Particle Texture", 2D) = "white" {}
		[HideInInspector]_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTexture("MainTexture", 2D) = "white" {}
		_Emision("Emision", Float) = 1
		_ProgressBar("ProgressBar", Range( 0 , 1)) = 1
		_DissolveEdge("DissolveEdge", Range( 0 , 1)) = -0.49
		_Noise("Noise", 2D) = "white" {}
		[Toggle(_MASK_OVER_ON)] _Mask_over("Mask_over", Float) = 0
		_OverMask("OverMask", 2D) = "white" {}
		_ColorMask("ColorMask", Color) = (1,1,1,1)
		_OverMaskEmission("OverMaskEmission", Float) = 0.3
		_OverMask_Speed_X("OverMask_Speed_X", Range( -1 , 1)) = 0
		_OverMask_Speed_Y("OverMask_Speed_Y", Range( -1 , 1)) = 0
		_OverMask_Tiling_X("OverMask_Tiling_X", Range( 0 , 5)) = 0
		_OverMask_Tiling_Y("OverMask_Tiling_Y", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#pragma shader_feature_local _MASK_OVER_ON


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _MainTexture;
				uniform float _OverMaskEmission;
				uniform sampler2D _OverMask;
				SamplerState sampler_OverMask;
				uniform float _OverMask_Speed_X;
				uniform float _OverMask_Speed_Y;
				uniform float _OverMask_Tiling_X;
				uniform float _OverMask_Tiling_Y;
				uniform float _Emision;
				uniform float4 _ColorMask;
				uniform float _DissolveEdge;
				uniform float _ProgressBar;
				uniform sampler2D _Noise;
				SamplerState sampler_Noise;
				uniform float4 _Noise_ST;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 texCoord50 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float4 tex2DNode14 = tex2D( _MainTexture, texCoord50 );
					float clampResult48 = clamp( _OverMask_Speed_X , -5.0 , 5.0 );
					float clampResult49 = clamp( _OverMask_Speed_Y , -5.0 , 5.0 );
					float2 appendResult39 = (float2(clampResult48 , clampResult49));
					float clampResult46 = clamp( _OverMask_Tiling_X , 0.0 , 5.0 );
					float clampResult47 = clamp( _OverMask_Tiling_Y , 0.0 , 5.0 );
					float2 appendResult45 = (float2(clampResult46 , clampResult47));
					float2 texCoord23 = i.texcoord.xy * appendResult45 + float2( 2,0 );
					float2 panner24 = ( 1.0 * _Time.y * appendResult39 + texCoord23);
					float temp_output_27_0 = saturate( ( _OverMaskEmission * tex2D( _OverMask, panner24 ).r ) );
					float4 temp_cast_0 = (temp_output_27_0).xxxx;
					float4 lerpResult29 = lerp( tex2DNode14 , temp_cast_0 , ( temp_output_27_0 * tex2DNode14 ));
					#ifdef _MASK_OVER_ON
					float4 staticSwitch31 = ( lerpResult29 * _Emision * _ColorMask );
					#else
					float4 staticSwitch31 = tex2DNode14;
					#endif
					float lerpResult54 = lerp( ( 0.98 + _DissolveEdge ) , ( 0.0 - _DissolveEdge ) , _ProgressBar);
					float2 texCoord2 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float smoothstepResult4 = smoothstep( ( lerpResult54 + ( _DissolveEdge * -1.5 ) ) , ( lerpResult54 + _DissolveEdge ) , ( 1.0 - texCoord2.x ));
					float temp_output_10_0 = saturate( smoothstepResult4 );
					float2 uv_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float lerpResult5 = lerp( tex2D( _Noise, uv_Noise ).r , temp_output_10_0 , temp_output_10_0);
					

					fixed4 col = ( staticSwitch31 * saturate( ( temp_output_10_0 * lerpResult5 ) ) );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18500
24.8;296.8;1160;506.2;2895.852;1404.373;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;44;-2366.26,-1201.97;Inherit;False;Property;_OverMask_Tiling_Y;OverMask_Tiling_Y;12;0;Create;True;0;0;False;0;False;0;0.53;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2368.523,-1323.077;Inherit;False;Property;_OverMask_Tiling_X;OverMask_Tiling_X;11;0;Create;True;0;0;False;0;False;0;0.45;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;47;-2036.049,-1196.334;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;46;-2037.363,-1317.151;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-2030.451,-868.0179;Inherit;False;Property;_OverMask_Speed_Y;OverMask_Speed_Y;10;0;Create;True;0;0;False;0;False;0;0.01;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-2035.738,-1005.844;Inherit;False;Property;_OverMask_Speed_X;OverMask_Speed_X;9;0;Create;True;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;49;-1681.391,-863.4147;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-5;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;48;-1676.533,-999.5858;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-5;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;45;-1692.363,-1252.535;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1455.992,-959.5705;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-3022.434,616.6227;Inherit;False;Property;_DissolveEdge;DissolveEdge;3;0;Create;True;0;0;False;0;False;-0.49;0.136;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-1493.867,-1256.182;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;10,2;False;1;FLOAT2;2,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-2290.3,574.4286;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-2266.069,926.4938;Inherit;False;2;2;0;FLOAT;0.98;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;-1188.528,-1191.625;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.81;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2356.537,427.6878;Inherit;False;Property;_ProgressBar;ProgressBar;2;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1697.79,-62.04333;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-925.4271,-976.0114;Inherit;False;Property;_OverMaskEmission;OverMaskEmission;8;0;Create;True;0;0;False;0;False;0.3;5.94;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-947.7473,-1238.734;Inherit;True;Property;_OverMask;OverMask;6;0;Create;True;0;0;False;0;False;-1;None;c6c55ca1157082f4e9a9979a74e9e04d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;54;-2012.957,583.3453;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1732.579,936.7169;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1420.704,463.5736;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-1480.429,652.9982;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-1081.024,-816.3423;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-583.8204,-1021.274;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;58;-1078.021,91.55106;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-811.58,-841.3478;Inherit;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;False;0;False;-1;29ff51df72b94ab449fc0666f9fd8d71;29ff51df72b94ab449fc0666f9fd8d71;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;4;-381.3747,17.708;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;27;-365.8573,-1051.652;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-259.0023,-734.9744;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-496.1727,554.375;Inherit;True;Property;_Noise;Noise;4;0;Create;True;0;0;False;0;False;-1;85b544dc6e75c0a4f891ca44a816b09a;beea0694d337e23449c7d21e4dfef1cb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;10;-229.5078,-11.02533;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;-50.5354,-984.416;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;5;-150.6931,277.819;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;142.9387,-553.6968;Inherit;False;Property;_Emision;Emision;1;0;Create;True;0;0;False;0;False;1;1.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;19;244.8988,-1022.605;Inherit;False;Property;_ColorMask;ColorMask;7;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;453.0176,-715.3633;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;115.5089,89.93906;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;31;488.9405,-383.1383;Inherit;False;Property;_Mask_over;Mask_over;5;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;16;394.6473,146.4366;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;813.0671,-104.038;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;42;1228.785,-91.12921;Float;False;True;-1;2;ASEMaterialInspector;0;7;ProgressBar_BorderNoise;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;47;0;44;0
WireConnection;46;0;43;0
WireConnection;49;0;38;0
WireConnection;48;0;36;0
WireConnection;45;0;46;0
WireConnection;45;1;47;0
WireConnection;39;0;48;0
WireConnection;39;1;49;0
WireConnection;23;0;45;0
WireConnection;61;1;7;0
WireConnection;62;1;7;0
WireConnection;24;0;23;0
WireConnection;24;2;39;0
WireConnection;25;1;24;0
WireConnection;54;0;62;0
WireConnection;54;1;61;0
WireConnection;54;2;11;0
WireConnection;53;0;7;0
WireConnection;56;0;54;0
WireConnection;56;1;53;0
WireConnection;55;0;54;0
WireConnection;55;1;7;0
WireConnection;28;0;26;0
WireConnection;28;1;25;1
WireConnection;58;0;2;1
WireConnection;14;1;50;0
WireConnection;4;0;58;0
WireConnection;4;1;56;0
WireConnection;4;2;55;0
WireConnection;27;0;28;0
WireConnection;30;0;27;0
WireConnection;30;1;14;0
WireConnection;10;0;4;0
WireConnection;29;0;14;0
WireConnection;29;1;27;0
WireConnection;29;2;30;0
WireConnection;5;0;13;1
WireConnection;5;1;10;0
WireConnection;5;2;10;0
WireConnection;20;0;29;0
WireConnection;20;1;18;0
WireConnection;20;2;19;0
WireConnection;6;0;10;0
WireConnection;6;1;5;0
WireConnection;31;1;14;0
WireConnection;31;0;20;0
WireConnection;16;0;6;0
WireConnection;15;0;31;0
WireConnection;15;1;16;0
WireConnection;42;0;15;0
ASEEND*/
//CHKSM=DAFB714FDF0372557ED8AB839A509A177B62B0B7