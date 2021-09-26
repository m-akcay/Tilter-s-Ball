Shader "Unlit/BlurredShader 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurStrength ("Blur Strength", float) = 0
        _CollisionSpeed ("Collision Speed", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "Universal"
        }

        LOD 100

        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half2 posUV : TEXCOORD1;
                half4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 _CameraOpaqueTexture_TexelSize;
            half _BlurStrength;
            half _CollisionSpeed;

            half3 GetBlurredScreenColor(in const half2 uv, half strength);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                o.posUV = ComputeScreenPos(o.vertex) / o.vertex.w;

                //o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col;// = half4(GetBlurredScreenColor(i.uv), 1);
                half strength = 1 - smoothstep(0, 1, distance(i.uv, half2(0.5f, 0.5f)));
                strength *= 5 * _CollisionSpeed;
                col = half4(GetBlurredScreenColor(i.posUV, strength), 1);
                //col = half4(SampleSceneColor(i.posUV), 1);
                return col;
            }

            half3 GetBlurredScreenColor(in const half2 uv, half strength)
            {
                #define OFFSET_X(kernel) half2(_CameraOpaqueTexture_TexelSize.x * kernel * strength, 0)
                #define OFFSET_Y(kernel) half2(0, _CameraOpaqueTexture_TexelSize.y * kernel * strength)

                #define BLUR_PIXEL(weight, kernel)  \
                    + (SampleSceneColor(uv + OFFSET_Y(kernel)) * weight) \
                    + (SampleSceneColor(uv - OFFSET_Y(kernel)) * weight) \
                    + (SampleSceneColor(uv + OFFSET_X(kernel)) * weight) \
                    + (SampleSceneColor(uv - OFFSET_X(kernel)) * weight) 
                   /* + (SampleSceneColor(uv + ((OFFSET_X(kernel) + OFFSET_Y(kernel)))) * weight * 0.125) \
                    + (SampleSceneColor(uv + ((OFFSET_X(kernel) - OFFSET_Y(kernel)))) * weight * 0.125) \
                    + (SampleSceneColor(uv - ((OFFSET_X(kernel) + OFFSET_Y(kernel)))) * weight * 0.125) \
                    + (SampleSceneColor(uv - ((OFFSET_X(kernel) - OFFSET_Y(kernel)))) * weight * 0.125) \*/

                half3 Sum = BLUR_PIXEL(0.25f, 1);

                //Sum += BLUR_PIXEL(0.05, 6);
                //Sum += BLUR_PIXEL(0.05, 5.5);

                //Sum += BLUR_PIXEL(0.065, 4.5);
                //Sum += BLUR_PIXEL(0.065, 4);
                
                /*Sum += BLUR_PIXEL(0.65, 0);
                Sum += BLUR_PIXEL(0.65, 1);

                Sum += BLUR_PIXEL(0.065, 2);
                Sum += BLUR_PIXEL(0.065, 3);*/


                #undef BLUR_PIXEL
                #undef OFFSET_X
                #undef OFFSET_Y

                return Sum;
            }

            ENDHLSL
        }
    }
}
