// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHDR_Dissovle"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.56
		_Albedo("Albedo", 2D) = "white" {}
		_SpeedNoiseDissolve("SpeedNoiseDissolve", Float) = 1
		_TillingDissolve("TillingDissolve", Vector) = (5,5,0,0)
		_Booster("Booster", Float) = 1
		[HDR]_DissolveGlowColor("DissolveGlow Color", Color) = (4,1.035294,0,0)
		_NormalMap("NormalMap", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		____DissolveValueVertex____("____DissolveValueVertex____", Float) = 0
		_Range("Range", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _DissolveGlowColor;
		uniform sampler2D _TextureSample0;
		SamplerState sampler_TextureSample0;
		uniform float2 _TillingDissolve;
		uniform float _SpeedNoiseDissolve;
		uniform float _Booster;
		uniform float ____DissolveValueVertex____;
		uniform float _Range;
		uniform float _Cutoff = 0.56;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = tex2D( _NormalMap, uv_NormalMap ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( tex2DNode1 * _Color ).rgb;
			float mulTime8 = _Time.y * _SpeedNoiseDissolve;
			float2 panner7 = ( mulTime8 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord2 = i.uv_texcoord * _TillingDissolve + panner7;
			float temp_output_10_0 = ( tex2D( _TextureSample0, uv_TexCoord2 ).b + _Booster );
			float Noise12 = temp_output_10_0;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform74 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float temp_output_16_0 = ( transform74.y + ____DissolveValueVertex____ );
			float Y_Gradient24 = saturate( ( temp_output_16_0 / _Range ) );
			float4 temp_output_48_0 = ( _DissolveGlowColor * ( Noise12 * Y_Gradient24 ) );
			float4 Emission40 = temp_output_48_0;
			o.Emission = ( Emission40 * Noise12 ).rgb;
			o.Alpha = 1;
			clip( ( 1.0 - ( Noise12 * Y_Gradient24 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
-78.4;477.6;1160;523;2621.8;319.5625;2.155261;True;False
Node;AmplifyShaderEditor.CommentaryNode;22;-2113.588,-340.5049;Inherit;False;1946.449;1044.034;Comment;10;9;8;7;6;2;4;11;10;5;78;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2063.588,374.7775;Inherit;True;Property;_SpeedNoiseDissolve;SpeedNoiseDissolve;2;0;Create;True;0;0;False;0;False;1;0.001;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;21;-2622.882,802.3059;Inherit;False;1647.874;706.4229;Comment;10;16;18;20;24;56;14;74;75;76;77;Y_Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;8;-1849.588,249.7775;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;14;-2611.278,863.5332;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;75;-2478.12,1299.818;Inherit;False;Property;____DissolveValueVertex____;____DissolveValueVertex____;8;0;Create;True;0;0;False;0;False;0;0.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;6;-1546.734,-290.5049;Inherit;True;Property;_TillingDissolve;TillingDissolve;3;0;Create;True;0;0;False;0;False;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;7;-1563.588,307.7775;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;74;-2180.639,1025.356;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1907.922,1046.856;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1169.991,19.22484;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;77;-1814.843,1372.396;Inherit;False;Property;_Range;Range;9;0;Create;True;0;0;False;0;False;0;1.56;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-821.4357,444.5293;Inherit;True;Property;_Booster;Booster;4;0;Create;True;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-841.5545,4.588799;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;False;0;False;-1;None;f485c52bfe0abf443890f2430fa1c426;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-1500.824,1145.19;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;20;-1379.426,972.7961;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-512.7565,410.3294;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-1248.551,1056.307;Inherit;True;Y_Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;1073.823,792.2648;Inherit;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;105.1282,73.20123;Inherit;False;24;Y_Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;175.857,-101.3119;Inherit;False;12;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;634.7056,-36.32114;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;47;656.1972,-329.803;Float;False;Property;_DissolveGlowColor;DissolveGlow Color;5;1;[HDR];Create;True;0;0;False;0;False;4,1.035294,0,0;0,48.41894,102.7558,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;13;2034.113,963.6229;Inherit;True;12;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;1045.91,-178.2631;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;1887.731,1399.242;Inherit;True;24;Y_Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;1945.6,-103.9644;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;False;-1;None;f7eb1e3a069534e468395ce94bd414ca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;2380.25,1199.776;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-792.4394,1399.925;Inherit;False;1355.212;637.3323;OpacityMask;10;30;27;28;26;29;33;31;32;34;35;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;1757.332,279.5302;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;54;2557.329,-126.9258;Inherit;False;Property;_Color;Color;7;0;Create;True;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;28;-504.9405,1682.556;Inherit;True;12;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;29;-491.1666,1449.925;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;110.7726,1794.45;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-64.22742,1914.45;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;76;-2258.55,841.2003;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-275.1757,1462.177;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;1893.563,573.3879;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;52;2574.473,1037.531;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-742.4394,1565.697;Inherit;False;24;Y_Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-734.5106,1818.768;Inherit;False;24;Y_Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;2754.501,396.1301;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;56;-1698.482,856.0731;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-8;False;2;FLOAT;8;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-438.4843,1902.257;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;2356.637,422.6438;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;53;2487.649,813.9042;Inherit;True;Property;_NormalMap;NormalMap;6;0;Create;True;0;0;False;0;False;-1;None;f7eb1e3a069534e468395ce94bd414ca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1459.824,51.29932;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;338.7726,1495.45;Inherit;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;2203.815,566.3301;Inherit;False;40;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;4;-946.7385,153.3333;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;1250.132,324.8109;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-71.22742,1718.45;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2941.888,684.8356;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SHDR_Dissovle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.56;True;True;0;True;Transparent;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;9;0
WireConnection;7;1;8;0
WireConnection;74;0;14;0
WireConnection;16;0;74;2
WireConnection;16;1;75;0
WireConnection;2;0;6;0
WireConnection;2;1;7;0
WireConnection;78;1;2;0
WireConnection;18;0;16;0
WireConnection;18;1;77;0
WireConnection;20;0;18;0
WireConnection;10;0;78;3
WireConnection;10;1;11;0
WireConnection;24;0;20;0
WireConnection;12;0;10;0
WireConnection;39;0;37;0
WireConnection;39;1;38;0
WireConnection;48;0;47;0
WireConnection;48;1;39;0
WireConnection;23;0;13;0
WireConnection;23;1;25;0
WireConnection;40;0;48;0
WireConnection;29;0;26;0
WireConnection;35;0;33;0
WireConnection;35;1;32;0
WireConnection;32;0;31;0
WireConnection;76;0;14;0
WireConnection;30;0;29;0
WireConnection;30;1;28;0
WireConnection;51;0;40;0
WireConnection;51;1;12;0
WireConnection;52;0;23;0
WireConnection;55;0;1;0
WireConnection;55;1;54;0
WireConnection;56;0;16;0
WireConnection;31;0;27;0
WireConnection;42;0;1;0
WireConnection;42;1;41;0
WireConnection;34;0;35;0
WireConnection;4;0;2;0
WireConnection;50;0;48;0
WireConnection;50;1;10;0
WireConnection;33;0;30;0
WireConnection;33;1;31;0
WireConnection;0;0;55;0
WireConnection;0;1;53;0
WireConnection;0;2;51;0
WireConnection;0;10;52;0
ASEEND*/
//CHKSM=C481C67BF505BCD7B4B1EA1269F6764CC1C2F559