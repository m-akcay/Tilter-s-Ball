Shader "Unlit/WallShader_MID"
{
    Properties
    {
        _NumberTextures("Tex", 2DArray) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
        
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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "NumberGenerator.hlsl"
            
            CBUFFER_START(UnityPerMaterial)

            TEXTURE2D_ARRAY(_NumberTextures);
            SAMPLER(sampler_NumberTextures);
                
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                //void sampleNumber_float(in SamplerState ss, in Texture2DArray arr, in float2 baseUV, in int level, out float4 outColor)

                float4 col;
                sampleNumber_float(sampler_NumberTextures, _NumberTextures, i.uv, 14, col);

                if (col.b > 0.001)
                    col = half4(0, 0, 0, 1);
                else
                    col = half4(1, 1, 1, 1);
                return col;
            }
            ENDHLSL
        }
    }
}
