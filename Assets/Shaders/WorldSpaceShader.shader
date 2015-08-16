Shader "Custom/WorldSpaceShader" {
	Properties {
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
		_MainTex("Texture",2D) = "white"{}

		_CenterPoint("Center",Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		//Tags{"LightMode"="Always"}

		
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		

		struct Input {
			half2 uv_MainTex;
			float3 customColor;
			float3 worldPos;
			
		};
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.customColor = abs(v.normal.y);
			o.uv_MainTex = v.texcoord;

		}

		

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

		void surf (Input IN, inout SurfaceOutput o) 
		{
			
			float dist = distance(_CenterPoint.xyz, IN.worldPos);

			if(dist>=_PeakLevel)
				o.Albedo = _PeakColor;
			if(dist<=_PeakLevel)
				o.Albedo = lerp(_Level3Color,_PeakColor,(dist-_Level3)/(_PeakLevel-_Level3));
			if(dist<=_Level3)
				o.Albedo = lerp(_Level2Color,_Level3Color,(dist-_Level2)/(_Level3-_Level2));
			if(dist<=_Level2)
				o.Albedo = lerp(_Level1Color,_Level2Color,(dist-_WaterLevel)/(_Level2-_WaterLevel));
			if(dist<=_WaterLevel)
				o.Albedo = _WaterColor;
			
			o.Albedo *=saturate(IN.customColor+_Slope);
			
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
