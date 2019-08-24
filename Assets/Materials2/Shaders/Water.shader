Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Opacity ("Opacity", Range(0,1)) = 0.0
		_Amplitude("Amplitude", Float) = .5
		_Frequency("Frequency", Float) = .5
		_WaveSpeed("WaveSpeed", Float) = .5
		_Offset("Offset", Vector) = (0, 0, 0, 0)
		_NormalSampleOffset("Normal Sample Offset", Float) = .01
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows alpha  vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _Opacity;
		float _Amplitude;
		float _Frequency;
		float _WaveSpeed;
		half2 _Offset;
		float _NormalSampleOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float fract(float x){
			return x - floor(x);
		}

		float3 mod(float a, float b){
			return a - b * floor(a/b);
		}

		float3 permute(float3 x) { return mod(((x*34.0)+1.0)*x, 289.0); }

		float snoise(float2 v){
		  const float4 C = float4(0.211324865405187, 0.366025403784439,
				   -0.577350269189626, 0.024390243902439);
		  float2 i  = floor(v + dot(v, C.yy) );
		  float2 x0 = v -   i + dot(i, C.xx);
		  float2 i1;
		  i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
		  float4 x12 = x0.xyxy + C.xxzz;
		  x12.xy -= i1;
		  i = mod(i, 289.0);
		  float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
		  + i.x + float3(0.0, i1.x, 1.0 ));
		  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy),
			dot(x12.zw,x12.zw)), 0.0);
		  m = m*m ;
		  m = m*m ;
		  float3 x = 2.0 * fract(p * C.www) - 1.0;
		  float3 h = abs(x) - 0.5;
		  float3 ox = floor(x + 0.5);
		  float3 a0 = x - ox;
		  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
		  float3 g;
		  g.x  = a0.x  * x0.x  + h.x  * x0.y;
		  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
		  return 130.0 * dot(m, g);
		}

		/*  For reference
		struct appdata_full {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		*/
	
		void vert (inout appdata_full v) {
			v.vertex.y =   _Amplitude * snoise(v.vertex.xz * _Frequency  +  _Time.y * _WaveSpeed);
			float3 offsetX =  float3(v.vertex.x + _NormalSampleOffset, _Amplitude * snoise(float2(v.vertex.x + _NormalSampleOffset, v.vertex.z) * _Frequency  +  _Time.y * _WaveSpeed), v.vertex.z);
			float3 offsetZ =  float3(v.vertex.x, _Amplitude * snoise(float2(v.vertex.x , v.vertex.z + _NormalSampleOffset) * _Frequency  +  _Time.y * _WaveSpeed), v.vertex.z + _NormalSampleOffset);
			float3 a = (offsetX - v.vertex.xyz);
			float3 b = (offsetZ - v.vertex.xyz);
			v.normal.xyz = normalize(cross(a, b));


		}

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Specular = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Opacity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
