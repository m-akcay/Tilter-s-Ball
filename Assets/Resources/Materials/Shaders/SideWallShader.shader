// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SideWallShader"
{
    Properties
    {
        _Color("_Color", Color) = (0, 0, 0, 0)
        ballPos("ballPos", Vector) = (0, 0, 0)
        thisWallPosX("_PosX", float) = 0
        thisWallPosY("_PosY", float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 200

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Util.hlsl"

            #define X_AXIS 0
            #define Y_AXIS 1
            #define Z_AXIS 2

            struct VertInput 
            {
                half4 vPos : POSITION;
            };

            struct FragInput 
            {
                half4 vPos : SV_POSITION;
                half3 worldPos : TEXCOORD1;
                half3 viewDir : TEXCOORD2;
            };

            half4 _Color;
            half4 _LevelColor;
            half4 _BallColor;
            half3 _Emission;

            half3 ballPos;
            half thisWallPosX;
           
            FragInput vert(VertInput v)
            {
                FragInput o;
                o.worldPos = mul(unity_ObjectToWorld, v.vPos);
                o.vPos = mul(UNITY_MATRIX_VP, half4(o.worldPos, 1));
                o.viewDir = -WorldSpaceViewDir(v.vPos);
                return o;
            }

            half4 frag(FragInput i) : SV_Target
            {
                half4 col = half4(1, 1, 1, 0);
                half distX = distance(ballPos.x, thisWallPosX);
                if (distX < 0.5)
                {
                    half distanceToBorder;
                    half alpha;
                    bool willShine = isCloseToBorder(i.worldPos.y, distanceToBorder);
                    if (willShine)
                    {
                        half smoothAlpha = (1 - distX) * (1 - smoothstep(0, 0.5, distanceToBorder));
                        col.a = 1 - smoothstep(0, smoothAlpha, distX);
                        half lerpVal;
                        if (ballWallDist(ballPos, i.worldPos, lerpVal))
                        {
                            lerpVal = smoothstep(0, 1, lerpVal);
                            col.rgb = lerp(_LevelColor.rgb, _BallColor.rgb, lerpVal);
                        }
                        else
                        {
                            col.rgb = _LevelColor.rgb;
                        }
                    }

                    col.a = clamp(col.a, 0.0, 1.0);
                }

                return col;
            }

            ENDHLSL
        }
    }
}

