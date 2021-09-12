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
        Tags { "RenderType" = "Opaque" }
        LOD 400

        Pass
        {
            HLSLPROGRAM

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
            float4 _MainTex_ST;

            float2 _ScreenWidthHeight;
            float4x4 _projMatrix;

            // w is the strength of the collision
            float4 _CollisionWithWall_WS;
            bool _ShouldBlur;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col;
                float2 uv = (i.vertex.xy / _ScreenWidthHeight) / 2;
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
                    col = tex2Dlod(_MainTex, float4(uv, 0, mipLevel));
                }
                else
                    col = tex2D(_MainTex, uv);
                
            //if (_ShouldBlur)
            //{
            //    float dist = distance(uv, _CollisionWithWall_WS.xy);
            //    float speedFactor = _CollisionWithWall_WS.w / 8;
            //    if (dist < speedFactor)
            //    {
            //        //col = tex2Dlod(_MainTex, float4(uv, 0, 1 + 2 * _CollisionWithWall_WS.w));
            //        //float mipLevel = 4 * smoothstep(0.0f, speedFactor, dist);

            //        col = tex2Dlod(_MainTex, float4(uv, 0, 4));
            //    }
            //}
                return col * 0.7f;
            }

            ENDHLSL
    }
    }
}
