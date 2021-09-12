// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SideWallShader_unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap("Normal Texture", 2D) = "bump" {}
        _Cube("Reflection Map", Cube) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

        #pragma target 3.0

        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _DiffuseTexture;
    //half4 _Cube_HDR;
    //UNITY_DECLARE_TEXCUBE(unity_SpecCube0);
    //UNITY_DECLARE_TEXCUBE(_Cube);
    samplerCUBE _Cube;
    sampler2D _NormalMap;
    float4 _NormalMap_ST;

    struct appdata
    {
        float4 vertex : POSITION;
        float4 normal : NORMAL;
        float2 texCoord : TEXCOORD0;
    };

    struct v2f {
        float4 pos : SV_POSITION;
        float2 coord: TEXCOORD0;
        float3 viewDir : TEXCOORD1;
    };

    v2f vert(appdata v) {
        v2f o;

        o.pos = UnityObjectToClipPos(v.vertex);
        o.viewDir = WorldSpaceViewDir(v.vertex);
        //o.coord = refract(viewDir, v.normal, 1 / 1.1f);
        //o.coord = reflect(viewDir, normal);
        o.coord = v.texCoord;

        return o;
    }

    float4 frag(v2f i) : SV_Target{
        float4 finalColor = 1.0;
        //finalColor = texCUBE(_Cube, coords) + float4(0.05f, 0.05f, 0.05f, 0.05f);
        float3 normal = UnpackNormal(tex2D(_NormalMap, i.coord));
        //float3 coords = reflect(i.viewDir, normal);
        float3 coords = normal;
        finalColor = texCUBElod(_Cube, float4(coords, 0.1f)) + float4(0.05f, 0.05f, 0.05f, 0.05f);
        return finalColor;
    }

        ENDCG
        }
    }
}
