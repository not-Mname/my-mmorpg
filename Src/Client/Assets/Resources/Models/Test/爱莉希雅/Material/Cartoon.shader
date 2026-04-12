//-----------------------------------------------【改进版卡通渲染Shader】----------------------------------------------
//     基于ToonComplete.shader改进，修复了光照、颜色混合和卡通感问题
//     保留了原有的自定义参数，但修复了关键计算问题
//---------------------------------------------------------------------------------------------------------------------

Shader "CartoonRender/Improved"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        // 环境光均匀应用于物体表面的所有部分。
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
        _AmbientAmount("Ambient Amount", Range(0, 5)) = 1
        [HDR]
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        // 控制高光反射的大小。
        _Glossiness("Glossiness", Float) = 32
        [HDR]
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.716
        // 控制边缘光在接近表面未照明部分时的平滑混合程度。
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
        _DiffuseAmount("Diffuse Amount", Range(0, 5)) = 1
        // 卡通感控制：值越小卡通感越强（明暗分界越明显）
        _ToonThreshold("Toon Threshold", Range(0.001, 0.1)) = 0.01

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
                "PassFlags" = "OnlyDirectional"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _AmbientColor;
            float4 _SpecularColor;
            float _Glossiness;
            float4 _RimColor;
            float _RimAmount;
            float _RimThreshold;
            float _AmbientAmount;
            float _DiffuseAmount;
            float _ToonThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                
                // 修正光照方向计算
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(normal, lightDir);
                
                // 改进的光照强度计算：使用更小的阈值增强卡通感
                float shadow = SHADOW_ATTENUATION(i);
                float lightIntensity = smoothstep(0, _ToonThreshold, NdotL * shadow);
                
                // 漫反射项：使用加法而不是乘法，避免颜色变暗
                float4 diffuse = lightIntensity * _LightColor0 * _DiffuseAmount;

                // 高光计算
                float3 halfVector = normalize(lightDir + viewDir);
                float NdotH = dot(normal, halfVector);
                float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
                float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
                float4 specular = specularIntensitySmooth * _SpecularColor;

                // 边缘光计算
                float rimDot = 1 - dot(viewDir, normal);
                float rimIntensity = rimDot * pow(saturate(NdotL), _RimThreshold);
                rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
                float4 rim = rimIntensity * _RimColor;

                // 环境光
                float4 ambient = _AmbientColor * _AmbientAmount;

                // 纹理采样
                float4 sample = tex2D(_MainTex, i.uv);

                // 改进的颜色混合：使用加法组合光照项，然后与颜色和纹理相乘
                float4 finalColor = (diffuse + ambient + specular + rim) * _Color * sample;
                return finalColor;
            }
            ENDCG
        }
        
        // 支持阴影投射
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
