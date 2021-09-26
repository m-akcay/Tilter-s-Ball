Shader "Unlit/BlurredShader"
{
    Properties
    {
        _MainTex("Render Texture", 2D) = "white" {}
        _ScreenWidthHeight("Screen Width", vector) = (0, 0, 0, 0)
        _CollisionWithWall_WS("Collision With Wall WS", vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "Universal"
        }

        LOD 400

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            #define TEXTURE2D_PARAMS(textureName, samplerName) sampler2D textureName
            //#define TEXTURE2D_ARGS(textureName, samplerName) sampler2D textureName
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@8.2/manual/writing-shaders-urp-unlit-texture.html
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float  _SampleScale;
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END


            float2 _ScreenWidthHeight;
            float4x4 _projMatrix;
            float4 _CameraOpaqueTexture_TexelSize;
            // w is the strength of the collision
            float4 _CollisionWithWall_WS;
            bool _ShouldBlur;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = ComputeScreenPos(v.vertex);
                o.uv.xy = float2(o.uv.x, 1 - o.uv.y);
                return o;
            }

            float4 UpsampleBox(float2 uv, float2 texelSize, float4 sampleScale)
            {
                float4 d = texelSize.xyxy * float4(-1.0, -1.0, 1.0, 1.0) * (sampleScale * 0.5);

                float4 s;
                s = ( SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv + d.xy)));
                s += (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv + d.zy)));
                s += (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv + d.xw)));
                s += (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv + d.zw)));

                return s * (1.0 / 4.0);
            };
            float4 UnityStereoAdjustedTexelSize(float4 texelSize) // Should take in _MainTex_TexelSize
            {
                texelSize.x = texelSize.x * 2.0; // texelSize.x = 1/w. For a double-wide texture, the true resolution is given by 2/w. 
                texelSize.z = texelSize.z * 0.5; // texelSize.z = w. For a double-wide texture, the true size of the eye texture is given by w/2. 
                return texelSize;
            }
            float4 frag(v2f i) : SV_Target
            {
                float4 col;
                float2 uv = (i.vertex.xy) / _ScreenWidthHeight;// / 2;
                _CollisionWithWall_WS.y = 1 + _CollisionWithWall_WS.y;
                // below values should only be calculated
                // if _ShouldBlur is true
                // it becomes broken if the (_ShouldBlur && xxx) condition changes to
                // if _ShouldBlue 
                //      if xxx
                // ?????????????
                // they should be same

                if (_ShouldBlur && 
                    distance(uv, _CollisionWithWall_WS.xy) < _CollisionWithWall_WS.w / 4)
                {
                    float mipLevel = 6 * (1 - (smoothstep(0.0f, _CollisionWithWall_WS.w / 4, distance(uv, _CollisionWithWall_WS.xy))));
                    //col = tex2Dlod(sampler_MainTex, float4(uv, 0, mipLevel));
                    col = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv, mipLevel);
                    //SAMPLE_TEXTURE2D_LOD(Texture, Sampler, UV, LOD);
                }
                else
                    col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // float4 UpsampleBox(TEXTURE2D_PARAMS(tex, samplerTex), float2 uv, float2 texelSize, float4 sampleScale)
                // half4 bloom = UpsampleBox(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), 
                //                              i.texcoord, 
                //                              UnityStereoAdjustedTexelSize(_MainTex_TexelSize).xy, _SampleScale);
                //col = UpsampleBox(uv / 2, UnityStereoAdjustedTexelSize(_MainTex_TexelSize).xy, _SampleScale);
                col = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, i.uv.xy / _CameraOpaqueTexture_TexelSize.xy);
                col.a = 0.4f;
                return col;
                //return col * 0.7f;
            }

            ENDHLSL
    }
    }
}
