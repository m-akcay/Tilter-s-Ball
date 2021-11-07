Shader "Unlit/WallShader_MID"
{
    Properties
    {
        _NumberTextures("Tex", 2DArray) = "" {}
        _BaseColor("Color", Color) = (1, 1, 1)
        showLevel("showLevel", int) = 0
        level("level", int) = 0
        /*_LightDirection("Light direction", Vector) = (0, 0, 1)
        _ViewPos("view pos", Vector) = (0, 0, 0)*/

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
                float4 positionWS : TEXCOORD1;
            };

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "NumberGenerator.hlsl"
            
            CBUFFER_START(UnityPerMaterial)

            TEXTURE2D_ARRAY(_NumberTextures);
            SAMPLER(sampler_NumberTextures);
            //uniform float3 _LightDirection;

            CBUFFER_END

            float3 simpleLighting(float3 surfaceColor, float3 positionWS);

            float4 _BaseColor;
            bool showLevel;
            int level;

            uniform float3 _LightDirection;
            uniform float3 _ViewPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.positionWS = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.positionWS);
                o.uv = v.uv;
                // convert normal to world space
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col;

                if (showLevel)
                {
                    sampleNumber_float(sampler_NumberTextures, _NumberTextures, i.uv, level, col);
                    if (col.b > 0.001)
                        col = 1 - _BaseColor;
                    else
                        col = _BaseColor;
                }
                else
                    col = _BaseColor;

                col = float4(simpleLighting(col, i.positionWS.xyz), 1);
                //col = _BaseColor;
                return col;
            }

            float3 simpleLighting(in float3 surfaceColor, in float3 positionWS)
            {
                float3 normal = float3(0, 0, -1);
                float diffuse = max(dot(normal, _LightDirection), 0.0) * 0.5;

                float specStr = 1;
                float3 viewDir = normalize(_ViewPos - positionWS);
                float3 reflectedDir = reflect(-_LightDirection, normal);
                float spec = pow(max(dot(viewDir, reflectedDir), 0.0), 64);
                float3 specColor = specStr * spec;

                return (diffuse + specColor) * surfaceColor;
            }

            ENDHLSL
        }
    }
}
