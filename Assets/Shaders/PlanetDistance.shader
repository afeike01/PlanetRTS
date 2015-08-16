Shader "Custom/PlanetDistance" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap",2D) = "bump"{}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_PeakColor ("PeakColor", Color) = (0.8,0.9,0.9,1)
		_PeakLevel ("PeakLevel", Float) = 300
		_Level3Color("Level3Color",Color) = (0.75,0.53,0,1)
		_Level3("Level3",Float) = 200
		_Level2Color("Level2Color", Color) = (0.69,0.63,0.31,1)
		_Level2("Level2",Float) = 100
		_Level1Color("Level1Color",Color)=(0.65,0.86,0.63,1)
		_WaterLevel("WaterLevel",Float)=0
		_WaterColor("WaterColor",Color)=(0.37,0.78,0.92,1)
		_Slope("Slope Fader",Range(0,1)) = 0
		_CenterPoint("Center",Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 customColor;
			float3 worldPos;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.customColor = abs(v.normal.y);
			o.uv_MainTex = v.texcoord;

		}

		sampler2D _MainTex;
		sampler2D _BumpMap;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _PeakLevel;
		float4 _PeakColor;
		float _Level3;
		float4 _Level3Color;
		float _Level2;
		float4 _Level2Color;
		float _Level1;
		float4 _Level1Color;
		float _Slope;
		float _WaterLevel;
		float4 _WaterColor;
		float4 _CenterPoint;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float dist = distance(_CenterPoint.xyz, IN.worldPos);

			if(dist>=_PeakLevel)
				_Color = _PeakColor;
			if(dist<=_PeakLevel)
				_Color = lerp(_Level3Color,_PeakColor,(dist-_Level3)/(_PeakLevel-_Level3));
			if(dist<=_Level3)
				_Color = lerp(_Level2Color,_Level3Color,(dist-_Level2)/(_Level3-_Level2));
			if(dist<=_Level2)
				_Color = lerp(_Level1Color,_Level2Color,(dist-_WaterLevel)/(_Level2-_WaterLevel));
			if(dist<=_WaterLevel)
				_Color = _WaterColor;
			
			o.Albedo *=saturate(IN.customColor+_Slope);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color*3;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
