Shader "Volumetric Audio/Ghost"
{
	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainColor("Main Color", Color) = (1.0,0.5,0.5,1.0)
		_RimColor("Rim Color", Color) = (0.5,1.0,0.5,1.0)
		_RimPower("Rim Power", Float) = 2.0
		_FadePower("Fade Power", Float) = 2.0
	}

	SubShader
	{
		Cull Off
		ZWrite Off

		CGPROGRAM
		#pragma surface Surf NoLighting alpha:fade

		float3 _Color;
		float3 _MainColor;
		float3 _RimColor;
		float  _RimPower;
		float  _FadePower;

		struct Input
		{
			float3 worldNormal;
			float3 worldRefl;
			float3 worldPos;
		};

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

		void Surf(Input i, inout SurfaceOutput o)
		{
			// Normalize vectors before use
			i.worldNormal = normalize(i.worldNormal);
			i.worldRefl   = normalize(i.worldRefl);

			// Find dot between normal and reflection vectors
			float nfDot = abs(dot(i.worldNormal, i.worldRefl));

			// Make the color a rim gradient
			float rim = 1.0f - pow(1.0f - nfDot, _RimPower);

			o.Albedo = lerp(_RimColor, _MainColor, rim) * _Color;

			// Fade alpha
			o.Alpha = 1.0f - pow(1.0f - nfDot, _FadePower);
		}
		ENDCG
	} // SubShader
} // Shader