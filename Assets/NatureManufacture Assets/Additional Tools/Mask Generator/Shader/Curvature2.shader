Shader "NatureManufacture Shaders/Debug/Curvature2"
{
    Properties
    {
        _MainTex ("Normal Map", 2D) = "white" {}
        _Scale ("Scale", Float ) = 0.5
        _NormalStrength ("Normal Strength", Float ) = 1
    }
    SubShader
    {
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
            float _Scale;
            float _NormalStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

          

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = UnpackNormalWithScale(tex2D(_MainTex, i.uv), _NormalStrength);
          
                // calculate curvature
                float3 derX = ddx(normal);
                float3 derY = ddy(normal);
                float3 xn = normal - derX;
                float3 xp = normal + derX;
                float3 yn = normal - derY;
                float3 yp = normal + derY;

                float s = tex2D(_MainTex, i.uv);
                float3 pos = float3(i.uv.x, i.uv.y, s);
                float mag = length(pos);


                float curvature = (cross(xn, xp).y - cross(yn, yp).x) * _Scale / mag;

                return fixed4((curvature + 0.5).xxx, 1);
            }
            ENDCG
        }
    }
}