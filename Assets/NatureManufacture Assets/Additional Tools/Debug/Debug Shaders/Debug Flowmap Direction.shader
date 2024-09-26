Shader "NatureManufacture Shaders/Debug/Flowmap Direction"
{
	Properties
	{
		_Direction("Direction", 2D) = "white" {}
		_Direction_Stop("Direction_Stop", 2D) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv4_texcoord4;
			float2 uv_texcoord;
		};

		uniform sampler2D _Direction_Stop;
		uniform sampler2D _Direction;


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float temp_output_77_0 = distance( i.uv4_texcoord4 , float2( 0,0 ) );
			Gradient gradient48 = NewGradient( 0, 3, 2, float4( 0, 0.0569272, 1, 0 ), float4( 1, 0.9791913, 0, 0.4882429 ), float4( 1, 0, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 uv_TexCoord51 = i.uv_texcoord * float2( 2,2 );
			float2 break15 = i.uv4_texcoord4;
			float temp_output_17_0 = ( break15.y * -1.0 );
			float temp_output_18_0 = atan( ( break15.x / temp_output_17_0 ) );
			float temp_output_20_0 = ( temp_output_17_0 > 0.0 ? temp_output_18_0 : ( temp_output_18_0 + 3.141592 ) );
			float cos76 = cos( temp_output_20_0 );
			float sin76 = sin( temp_output_20_0 );
			float2 rotator76 = mul( frac( ( uv_TexCoord51 / ( float2( 1,1 ) / float2( 20,20 ) ) ) ) - float2( 0.5,0.5 ) , float2x2( cos76 , -sin76 , sin76 , cos76 )) + float2( 0.5,0.5 );
			float2 normalizeResult5 = normalize( i.uv4_texcoord4 );
			float2 uv_TexCoord23 = i.uv_texcoord * float2( 2,2 ) + ( ( _Time.y * ( round( ( ( normalizeResult5 * (0.5 + (length( i.uv4_texcoord4 ) - 0.0) * (1.0 - 0.5) / (1.0 - 0.0)) ) * 5.0 ) ) / 5.0 ) ) * float2( 0.1,0.1 ) );
			float cos40 = cos( temp_output_20_0 );
			float sin40 = sin( temp_output_20_0 );
			float2 rotator40 = mul( frac( ( uv_TexCoord23 / ( float2( 1,1 ) / float2( 20,20 ) ) ) ) - float2( 0.5,0.5 ) , float2x2( cos40 , -sin40 , sin40 , cos40 )) + float2( 0.5,0.5 );
			o.Emission = ( temp_output_77_0 == 0.0 ? ( SampleGradient( gradient48, 0.0 ) * tex2D( _Direction_Stop, rotator76 ).a ) : ( tex2D( _Direction, rotator40 ).a * SampleGradient( gradient48, temp_output_77_0 ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}