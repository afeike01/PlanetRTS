Shader "Custom/Distance" {
	Properties {
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MinColor("Color in Minimal", Color) = (1,1,1,1)
		_MaxColor("Color in Maximal", Color) = (0,0,0,0)
		_MinDistance("Min Distance", Float) = 100
		_MaxDistance("Max Distance", Float) = 1000
		_CenterPoint("Center",Vector) = (0,0,0,0)
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float _MaxDistance;
		float _MinDistance;
		half4 _MinColor;
		half4 _MaxColor;
		float4 _CenterPoint;

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			float dist = distance(_CenterPoint.xyz, IN.worldPos);
			half weight = saturate((dist-_MinDistance)/(_MaxDistance - _MinDistance));
			half4 distanceColor = lerp(_MinColor,_MaxColor, weight);

			o.Albedo = c.rgb*distanceColor.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
