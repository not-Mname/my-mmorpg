Shader "Projector/SkillIndicator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("main color", color) = (1,1,1,1)
        _AddColor ("add color", color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            ColorMask RGB
            //混合公式 最终颜色 = 目标颜色 × (源颜色 + 1)当前使用黑白图，白色为图案，黑色为透明区域，
            //所以这里使用DstColor One，如果是黑色 最终颜色 = 目标颜色 × (0 + 1)，黑色就是透明的
            Blend DstColor One
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 shadow : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainColor, _AddColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.shadow = mul(unity_Projector, v.vertex);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.shadow));
                col.rgb *=  _MainColor.rgb * _MainColor.a;
                col.rgb += _AddColor * col * _AddColor.a;
                //col.a =  1 - col.a;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}