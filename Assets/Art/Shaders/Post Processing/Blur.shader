Shader "PostProcessing/Blur"
{
    Properties
    {
        _Spread ("Standard Deviation (Spread)", Float) = 0
        _GridSize ("Grid Size", Integer) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always ZWrite Off Cull Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #define E 2.71828f

        float _Spread;
        int _GridSize;

        struct VertexToFragment
        {
            float4 PositionCS : SV_POSITION;
            float2 Texcoord : TEXCOORD0;
        };

        VertexToFragment Vertex(Attributes input)
        {
            VertexToFragment output;
            output.PositionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
            output.Texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
            return output;
        }

        float Gaussian(int x)
        {
            float sigmaSqu = _Spread * _Spread;
            return 1 / sqrt(TWO_PI * sigmaSqu) * pow(E, -(x * x) / (2 * sigmaSqu));
        }
        ENDHLSL

        Pass
        {
            Name "Horizontal"

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            float4 Fragment(VertexToFragment input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 texelSize = _BlitTexture_TexelSize.xy;

                float3 col = float3(0.0f, 0.0f, 0.0f);
                float gridSum = 0.0f;

                int upper = (_GridSize - 1) / 2;
                int lower = -upper;

                for (int x = lower; x <= upper; ++x)
                {
                    float gauss = Gaussian(x);
                    gridSum += gauss;
                    float2 uv = input.Texcoord + float2(texelSize.x * x, 0.0f);
                    col += gauss * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).xyz;
                }

                col /= gridSum;
                return float4(col, 1.0f);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Vertical"

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            float4 Fragment(VertexToFragment input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 texelSize = _BlitTexture_TexelSize.xy;

                float3 col = float3(0.0f, 0.0f, 0.0f);
                float gridSum = 0.0f;

                int upper = (_GridSize - 1) / 2;
                int lower = -upper;

                for (int y = lower; y <= upper; ++y)
                {
                    float gauss = Gaussian(y);
                    gridSum += gauss;
                    float2 uv = input.Texcoord + float2(0.0f, texelSize.y * y);
                    col += gauss * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).xyz;
                }
                col /= gridSum;
                return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}