// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SideWallShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Emission("Emission", Color) = (0, 0, 0)
        _NormalMap("Normal Texture", 2D) = "bump" {}
        _EnvMap("Reflection Map", Cube) = "" {}

        
        ballPos("ballPos", Vector) = (0, 0, 0)
        thisWallPosX("_PosX", Float) = 0
        thisWallPosY("_PosY", Float) = 0

        level("_Level", Int) = 1
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 200

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define X_AXIS 0
            #define Y_AXIS 1
            #define Z_AXIS 2

            struct VertInput 
            {
                float4 vPos : POSITION;
                float2 texCoord : TEXCOORD0;
            };

            struct FragInput 
            {
                float4 vPos : SV_POSITION;
                float2 texCoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            samplerCUBE _EnvMap;

            float4 _Color;
            float3 _Emission;

            float3 ballPos;
            float thisWallPosX;

            bool isCloseToBorder(in float worldPosY, out float distanceToBorder);
            bool ballWallDist(in float3 ballPos, in float3 worldPos, inout float alpha);
            
            FragInput vert(VertInput v)
            {
                FragInput o;
                o.worldPos = mul(unity_ObjectToWorld, v.vPos);
                o.vPos = mul(UNITY_MATRIX_VP, float4(o.worldPos, 1));
                o.texCoord = float2(-v.texCoord.y, v.texCoord.x);
                //o.texCoord = TRANSFORM_TEX(v.texCoord, _MainTex);
                o.viewDir = -WorldSpaceViewDir(v.vPos);
                return o;
            }

            float4 frag(FragInput i) : SV_Target
            {
                float4 col = _Color;

                float distX = distance(ballPos.x, thisWallPosX);
                if (distX < 0.3f)
                {
                    float distanceToBorder;
                    bool willShine = isCloseToBorder(i.worldPos.y, distanceToBorder);
                    if (willShine)
                    {
                        float smoothAlpha = 1 - smoothstep(0, 0.4f, distanceToBorder);
                        //col.a = (1 - distX * 3) * 1.5f * smoothAlpha;
                        col.a = smoothstep(0.3f, 0.05f, distX) * 1.5f * smoothAlpha;
                    }

                    if (ballWallDist(ballPos, i.worldPos, col.a))
                    {
                        col.r = 0.5f;
                        col.b = 0;
                    }

                    col.a = clamp(col.a, 0.0f, 1.0f);
                }

                return col;
            }

            bool isCloseToBorder(in float worldPosY, out float distanceToBorder)
            {
                float distY = distance(worldPosY, 0);
                if (distY < 0.4f)
                {
                    distanceToBorder = distY;
                    return true;
                }
                else if (distY > 0.6f)
                {
                    distanceToBorder = 1 - distY;
                    return true;
                }

                return false;
            }

            bool ballWallDist(in float3 ballPos, in float3 worldPos, inout float alpha)
            {
                float radius = 0.2f;
                float distX = distance(ballPos.x, worldPos.x);
                if (distX < 0.25f)
                {
                    float distY = distance(ballPos.y, worldPos.y);
                    float distZ = distance(ballPos.z, worldPos.z);

                    float distToCenter = (distY * distY + distZ * distZ);
                    if (distToCenter > 0.04f)
                        return false;

                    alpha += 1 - smoothstep(0, clamp(radius + 0.05f - distX, 0, 1), distToCenter * 3);

                    return true;
                }
                
                return false;
            }


            ENDCG
        }
    }
}

