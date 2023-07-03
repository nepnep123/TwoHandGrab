Shader "Luciano/SonarUV" {
	Properties {
		_MainTex("Textura", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
		_RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
		_Linha("Linha", Range(0,1)) = 0.0
		_Tamanho("Tamanho", Range(0, 1)) = 0.1
		_Local("Local", Range(0, 1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
			
		ZTest Always
			
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 viewDir;
		};

		float4 _RimColor;
		float _RimPower;
		float _Tamanho;
		float _Linha;
		float _Local;

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

			float aux = IN.uv_MainTex.y * _Local + screenUV.y * (1 - _Local);

			float condicao = ceil(aux - _Linha) * (1 - ceil(aux - (_Linha + _Tamanho)));

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower) * condicao;
			o.Alpha = (pow(rim, _RimPower) * condicao) + (pow(rim, _RimPower * 4) * _Linha * (1 - condicao));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
