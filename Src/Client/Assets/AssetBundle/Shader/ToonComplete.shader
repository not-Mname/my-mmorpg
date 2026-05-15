
//-----------------------------------------------【Shader说明】----------------------------------------------
//     Shader功能： 卡通渲染
//     使用语言：   Shaderlab
//     开发所用IDE版本：Unity2018.3.6 、Visual Studio 2017
//     2019年4月10日  Created by Aladdin(阿拉丁)   
//     更多内容或交流请访问我的博客：http://dingxiaowei.cn
//---------------------------------------------------------------------------------------------------------------------

Shader "Test/Toon Complete"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		// 环境光均匀应用于物体表面的所有部分。
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		// 控制高光反射的大小。
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		// 控制边缘光在接近表面未照明部分时的平滑混合程度。
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1		
	}
	SubShader
	{
		Pass
		{
			// 设置我们的Pass使用正向渲染，并且只接收主方向光和环境光的数据。
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// 根据光照设置编译此着色器的多个版本。
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			// 下面的文件包含辅助光照和阴影计算的宏和函数。
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;	
				// 在 AutoLight.cginc 中找到的宏。根据平台目标的不同精度，
				// 将 vector4 声明到 TEXCOORD2 语义中。
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);		
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// 定义在 AutoLight.cginc 中。通过将顶点从世界空间变换到阴影贴图空间，
				// 来分配上述阴影坐标。
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 _AmbientColor;

			float4 _SpecularColor;
			float _Glossiness;		

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;	

			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				float NdotL = dot(_WorldSpaceLightPos0, normal);

				// 采样阴影贴图，返回 0...1 范围内的值，
				// 其中 0 表示在阴影中，1 表示不在阴影中。
				float shadow = SHADOW_ATTENUATION(i);
				// 将强度划分为亮部和暗部，并在两者之间平滑插值以避免生硬的断裂。
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);	
				// 乘以主方向光的强度和颜色。
				float4 light = lightIntensity * _LightColor0;

				// 计算镜面反射（高光）。
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				// 将 _Glossiness 自乘，以便美术人员在检查器中使用较小的高光值。
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;				

				// 计算边缘光。
				float rimDot = 1 - dot(viewDir, normal);
				// 我们只希望边缘光出现在表面的受光侧，
				// 因此将其乘以 NdotL，并进行幂运算以平滑混合。
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, i.uv);

				return (light + _AmbientColor + specular + rim) * _Color * sample;
			}
			ENDCG
		}

		// 支持阴影投射。
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}