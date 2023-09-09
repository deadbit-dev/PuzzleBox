Shader "Shape"
{
    Properties
    {
        [HideInInspector] [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Outline_Color("Outline Color", Color) = (0, 0, 0, 1)
        _Outline_Thickness("Outline Thickness", Range(0, 1)) = 0.3
        _Radius("Radius", Range(0, 1)) = 0.5
        _Segments("Segments", Float) = 0
        _DashLength("DashLength", Float) = 0.5
        _GapLength("GapLength", Float) = 0.5
    }
    SubShader
    {   
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "DisableBatching" = "True"
        }

        Pass
        {   
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off
            
            HLSLPROGRAM
            
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "utils.hlsl"

            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial);
                float4 _Color;
                float _Radius;
                float _Outline_Thickness;
                float4 _Outline_Color;
                float _Segments;
                float _DashLength;
                float _GapLength;
            CBUFFER_END

            v2f vert (appdata vertex)
            {
                v2f output;
                output.position = mul(UNITY_MATRIX_MVP, vertex.position);

                const float2 size = float2(
                    length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                    length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y))
                );
                
                output.uv = vertex.uv;
                output.uv = output.uv - float2(0.5, 0.5);
                output.uv = output.uv / (1 / size);
                output.uv = output.uv + float2(0.5, 0.5);
                
                return output;
            }

            half4 frag (v2f i) : SV_Target
            {
                const float2 size = float2(
                    length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                    length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y))
                );

                const float out_rectangle = rounded_rectangle(i.uv, size.x, size.y, _Radius, true);
                const float in_rectangle = rounded_rectangle(i.uv, size.x - _Outline_Thickness, size.y - _Outline_Thickness, _Radius - _Outline_Thickness * 0.5, true);
                
                float outline; // = out_rectangle - in_rectangle;
                outline_for_rounded_rectangle(i.uv, size.x, size.y, _Radius, _Outline_Thickness, _Segments, _DashLength, _GapLength, outline);
                const float4 colorized_outline = outline * _Outline_Color;

                const float4 colorized_shape = in_rectangle * _Color;
                
                return colorized_shape + colorized_outline;
            }
            
            ENDHLSL
        }
    }
}