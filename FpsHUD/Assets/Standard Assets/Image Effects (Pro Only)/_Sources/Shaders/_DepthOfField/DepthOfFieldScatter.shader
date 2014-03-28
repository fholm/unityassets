 Shader "Hidden/Dof/DepthOfFieldHdr" {
	Properties {
		_MainTex ("-", 2D) = "black" {}
		_FgOverlap ("-", 2D) = "black" {}
		_LowRez ("-", 2D) = "black" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half2 uv1 : TEXCOORD1;
	};

	struct v2fRadius {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv1[4] : TEXCOORD1;
	};
	
	struct v2fBlur {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4; 
		half4 uv89 : TEXCOORD5;
	};	
	
	uniform sampler2D _MainTex;
	uniform sampler2D _CameraDepthTexture;
	uniform sampler2D _FgOverlap;
	uniform sampler2D _LowRez;
	uniform half4 _CurveParams;
	uniform float4 _MainTex_TexelSize;	
	uniform half4 _Offsets;

	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv1.xy = v.texcoord.xy;
		o.uv.xy = v.texcoord.xy;
		
		#if SHADER_API_D3D9 || SHADER_API_XBOX360 || SHADER_API_D3D11
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif			
		
		return o;
	} 

	v2fBlur vertBlurPlusMinus (appdata_img v) {
		v2fBlur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1) * _MainTex_TexelSize.xyxy;
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * half4(2,2, -2,-2) * _MainTex_TexelSize.xyxy;// * 3.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * half4(3,3, -3,-3) * _MainTex_TexelSize.xyxy;// * 3.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * half4(4,4, -4,-4) * _MainTex_TexelSize.xyxy;// * 4.0;
		o.uv89 =  v.texcoord.xyxy + _Offsets.xyxy * half4(5,5, -5,-5) * _MainTex_TexelSize.xyxy;// * 5.0;
		return o;  
	}
		
	inline half3 Axis(half2 coords, half minRange)
	{
		half coc = tex2D(_MainTex, coords).a;
		coc *= coc;
		return half3( max(coc, minRange) * (_Offsets.xy), coc);
	}	

	inline half3 AxisFromSample(half4 sample, half minRange)
	{
		return half3( max(sample.a, minRange) * (_Offsets.xy), sample.a);
	}	
	
	inline half2 AxisFromSamplePoisson(half4 sample, half minRange)
	{
		return half2( max(sample.a, minRange), sample.a);
	}
	
	inline float4 AdjustForLowRezBuffers(half2 coords, half4 returnValue) 
	{		
		half4 highRezColor = tex2D(_MainTex, coords);
		return lerp(highRezColor, returnValue, smoothstep(0.135, 0.5, highRezColor.a)); // lerp value is important as blending HR <-> LR is pretty PRETTY REALLY ugly :/
	}
	
	inline float ForegroundOverlap(float2 coords)
	{
		return saturate(tex2D(_FgOverlap,coords).a);
	}
	
	inline float BokehWeightForDisc(float4 sample, float4 center, float blurWidth, float2 tapOffset)
	{ 
		return smoothstep(-1,0, sample.a * blurWidth - length(tapOffset * blurWidth * center.a));
	}
	inline float BokehWeightForDiscFg(float4 sample, float4 center, float blurWidth, float2 tapOffset)
	{ 
		return smoothstep(-1,0, length(tapOffset * blurWidth * center.a)- sample.a * blurWidth);
	}
			
	static const float2 poisson[13] =
	{
		float2(0,0), 
		float2(-0.326212,-0.40581),
		float2(-0.840144,-0.07358),
		float2(-0.695914,0.457137),
		float2(-0.203345,0.620716),
		float2(0.96234,-0.194983),
		float2(0.473434,-0.480026),
		float2(0.519456,0.767022),
		float2(0.185461,-0.893124),
		float2(0.507431,0.064425),
		float2(0.89642,0.412458),
		float2(-0.32194,-0.932615),
		float2(-0.791559,-0.59771)
	};

	half4 fragBlurPoisson (v2f i) : COLOR 
	{
		const int TAPS = 13; // X1	
		
		half4 centerTap = tex2D(_MainTex, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w;
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=1; l < TAPS; l++)
		{
			half2 sampleUV = i.uv1.xy + poisson[l].xy * poissonScale;
			half4 sample0 = tex2D(_MainTex, sampleUV.xy);	
								
			half weight0 = BokehWeightForDisc(sample0, centerTap, _Offsets.w, poisson[l].xy);

			sum += sample0 * weight0; 
			sampleCount += weight0 ; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;
		
		return returnValue;			
	}

	half4 fragBlurPoissonLowRez (v2f i) : COLOR 
	{
		const int TAPS = 13; // X1
		
		half4 centerTap = tex2D(_LowRez, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w;
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=1; l < TAPS; l++)
		{
			half2 sampleUV = i.uv1.xy + poisson[l].xy * poissonScale;
			half4 sample0 = tex2D(_LowRez, sampleUV.xy);	
								
			half weight0 = BokehWeightForDisc(sample0, centerTap, _Offsets.w, poisson[l].xy);

			sum += sample0 * weight0; 
			sampleCount += weight0 ; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;
		
		return AdjustForLowRezBuffers(i.uv1.xy, returnValue);		
	}	 
	 	 	 
	half4 fragBlurProduction (v2f i) : COLOR 
	{
		const int TAPS = 13; // X2	
				
		half4 centerTap = tex2D(_MainTex, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w;
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=1; l < TAPS; l++)
		{
			half4 sampleUV = i.uv1.xyxy + half4(poisson[l].xy,-poisson[l].xy) * poissonScale;
			
			half4 sample0 = tex2D(_MainTex, sampleUV.xy);	
			half4 sample1 = tex2D(_MainTex, sampleUV.zw);	 
								
			half weight0 = BokehWeightForDisc(sample0, centerTap, _Offsets.w, poisson[l].xy);
			half weight1 = BokehWeightForDisc(sample1, centerTap, _Offsets.w, -poisson[l].xy);

			sum += sample0 * weight0; 
			sum += sample1 * weight1; 
			sampleCount += weight0 + weight1; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;
		
		return returnValue;	
	}	

	half4 fragBlurProductionLowRez (v2f i) : COLOR 
	{
		const int TAPS = 13; // X2	
		
		half4 centerTap = tex2D(_LowRez, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w; 
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=1; l < TAPS; l++)
		{
			half4 sampleUV = i.uv1.xyxy + half4(poisson[l].xy,-poisson[l].xy) * poissonScale;
			
			half4 sample0 = tex2D(_LowRez, sampleUV.xy);	
			half4 sample1 = tex2D(_LowRez, sampleUV.zw);	 
								
			half weight0 = BokehWeightForDisc(sample0, centerTap, _Offsets.w, poisson[l].xy);
			half weight1 = BokehWeightForDisc(sample1, centerTap, _Offsets.w, -poisson[l].xy);

			sum += sample0 * weight0;
			sum += sample1 * weight1;
			
			sampleCount += weight0 + weight1; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;
		
		return AdjustForLowRezBuffers(i.uv1.xy, returnValue);	
	}		
	
	static const float3 movieTaps[60] =
	{
		float3(  0.2165,  0.1250, 1.0000 ),
		float3(  0.0000,  0.2500, 1.0000 ),
		float3( -0.2165,  0.1250, 1.0000 ),
		float3( -0.2165, -0.1250, 1.0000 ),
		float3( -0.0000, -0.2500, 1.0000 ),
		float3(  0.2165, -0.1250, 1.0000 ),
		float3(  0.4330,  0.2500, 1.0000 ),
		float3(  0.0000,  0.5000, 1.0000 ),
		float3( -0.4330,  0.2500, 1.0000 ),
		float3( -0.4330, -0.2500, 1.0000 ),
		float3( -0.0000, -0.5000, 1.0000 ),
		float3(  0.4330, -0.2500, 1.0000 ),
		float3(  0.6495,  0.3750, 1.0000 ),
		float3(  0.0000,  0.7500, 1.0000 ),
		float3( -0.6495,  0.3750, 1.0000 ),
		float3( -0.6495, -0.3750, 1.0000 ),
		float3( -0.0000, -0.7500, 1.0000 ),
		float3(  0.6495, -0.3750, 1.0000 ),
		float3(  0.8660,  0.5000, 1.0000 ),
		float3(  0.0000,  1.0000, 1.0000 ),
		float3( -0.8660,  0.5000, 1.0000 ),
		float3( -0.8660, -0.5000, 1.0000 ),
		float3( -0.0000, -1.0000, 1.0000 ),
		float3(  0.8660, -0.5000, 1.0000 ),
		float3(  0.2163,  0.3754, 0.8670 ),
		float3( -0.2170,  0.3750, 0.8670 ),
		float3( -0.4333, -0.0004, 0.8670 ),
		float3( -0.2163, -0.3754, 0.8670 ),
		float3(  0.2170, -0.3750, 0.8670 ),
		float3(  0.4333,  0.0004, 0.8670 ),
		float3(  0.4328,  0.5004, 0.8847 ),
		float3( -0.2170,  0.6250, 0.8847 ),
		float3( -0.6498,  0.1246, 0.8847 ),
		float3( -0.4328, -0.5004, 0.8847 ),
		float3(  0.2170, -0.6250, 0.8847 ),
		float3(  0.6498, -0.1246, 0.8847 ),
		float3(  0.6493,  0.6254, 0.9065 ),
		float3( -0.2170,  0.8750, 0.9065 ),
		float3( -0.8663,  0.2496, 0.9065 ),
		float3( -0.6493, -0.6254, 0.9065 ),
		float3(  0.2170, -0.8750, 0.9065 ),
		float3(  0.8663, -0.2496, 0.9065 ),
		float3(  0.2160,  0.6259, 0.8851 ),
		float3( -0.4340,  0.5000, 0.8851 ),
		float3( -0.6500, -0.1259, 0.8851 ),
		float3( -0.2160, -0.6259, 0.8851 ),
		float3(  0.4340, -0.5000, 0.8851 ),
		float3(  0.6500,  0.1259, 0.8851 ),
		float3(  0.4325,  0.7509, 0.8670 ),
		float3( -0.4340,  0.7500, 0.8670 ),
		float3( -0.8665, -0.0009, 0.8670 ),
		float3( -0.4325, -0.7509, 0.8670 ),
		float3(  0.4340, -0.7500, 0.8670 ),
		float3(  0.8665,  0.0009, 0.8670 ),
		float3(  0.2158,  0.8763, 0.9070 ),
		float3( -0.6510,  0.6250, 0.9070 ),
		float3( -0.8668, -0.2513, 0.9070 ),
		float3( -0.2158, -0.8763, 0.9070 ),
		float3(  0.6510, -0.6250, 0.9070 ),
		float3(  0.8668,  0.2513, 0.9070 )
	};

	// insane movie quality
	half4 fragBlurMovie (v2f i) : COLOR 
	{
		const int TAPS = 60;
					
		half4 centerTap = tex2D(_MainTex, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * (centerTap.a) * _Offsets.w;
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=0; l < TAPS; l++)
		{
			half2 sampleUV = i.uv1.xy + movieTaps[l].xy * poissonScale;
			half4 sample0 = tex2D(_MainTex, sampleUV.xy);	 
			half weight0 = saturate( BokehWeightForDisc(sample0, centerTap, _Offsets.w, movieTaps[l].xy) * movieTaps[l].z);
			sum += sample0 * weight0; 
			sampleCount += weight0; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;

		return returnValue;
	}	

	// insane movie quality in low resolution
	half4 fragBlurMovieLowRez (v2f i) : COLOR 
	{
		const int TAPS = 60;
					
		half4 centerTap = tex2D(_LowRez, i.uv1.xy);
		half4 sum = centerTap;
		half4 poissonScale = _MainTex_TexelSize.xyxy * (centerTap.a) * _Offsets.w; 
					
		half sampleCount = centerTap.a; 
		sum *= sampleCount;
		
		for(int l=1; l < TAPS; l++)
		{
			half2 sampleUV = i.uv1.xy + movieTaps[l].xy * poissonScale;
			half4 sample0 = tex2D(_LowRez, sampleUV.xy);	 					
			half weight0 = BokehWeightForDisc(sample0, centerTap, _Offsets.w, movieTaps[l].xy) * movieTaps[l].z;
			sum += sample0 * weight0; 
			sampleCount += weight0; 
		}
		
		half4 returnValue = sum / (0.00001 + sampleCount);	
		returnValue.a = centerTap.a;
		
		return AdjustForLowRezBuffers(i.uv1.xy, returnValue);
	}				
									
	half4 fragBlurHighSampleCount (v2f i) : COLOR 
	{
		const int TAPS = 12;
		const half FILTER_KERNEL_WEIGHTS[12] = {1.0, 0.8, 0.65, 0.5, 0.4, 0.2, 0.1, 0.05, 0.025, 0.0125, 0.005, 0.00175}; 
		//const half FILTER_KERNEL_WEIGHTS[10] = {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0};
		
		half fgOverlap = saturate(tex2D(_FgOverlap, i.uv1.xy).a*3); // CheckForegroundOverlap(i.uv.xy);
		
		
		half4 centerTap = tex2D(_MainTex, i.uv1.xy);
		half4 sum = centerTap;// * FILTER_KERNEL_WEIGHTS[0];
		
		half2 offset = AxisFromSample(sum, fgOverlap); //GetFirstAxis(TexCoord);
		half amount = length(offset.xy);				
								
	//	half centerDepth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv.xy).x);
				
		half sampleCount = FILTER_KERNEL_WEIGHTS[0];
		half4 steps = (offset.xy * _MainTex_TexelSize.xy).xyxy;
		steps.zw *= -1; 
		for(int l=1; l < TAPS; l++)
		{
			//djhkjsdhkjashdkjashdkj kashd  as iiiiii
			half4 sampleUV = i.uv1.xyxy + steps * l;
			
			// Color samples
			half4 sample0 = tex2D(_MainTex, sampleUV.xy);
			half4 sample1 = tex2D(_MainTex, sampleUV.zw);
			
			// Maximum extent of the blur at these samples
			half maxLengthAt0;
			half maxLengthAt1;
			
			maxLengthAt0 = length(AxisFromSample(sample0, fgOverlap).xy) * (TAPS+1);//length(GetFirstAxis(sampleUV.xy)) * (NUM_STEPS+1);
			maxLengthAt1 = length(AxisFromSample(sample1, fgOverlap).xy) * (TAPS+1);//length(GetFirstAxis(sampleUV.zw)) * (NUM_STEPS+1);

	//	half depth0 = Linear01Depth(tex2D(_CameraDepthTexture, sampleUV.xy).x);
		//half depth1 = Linear01Depth(tex2D(_CameraDepthTexture, sampleUV.zw).x);
		
		//depth0 = saturate(1-(centerDepth-depth0)*(centerDepth-depth0)*10.5);
		//depth1 = saturate(1-(centerDepth-depth1)*(centerDepth-depth1)*10.5);

			// Y U NO WORKY ?
			half currentLength = amount * ((half)l);
			
			half weight0 = max(0, saturate((maxLengthAt0 - currentLength))) * FILTER_KERNEL_WEIGHTS[l];//* depth0;
			sum += sample0 * weight0; 
			sampleCount += weight0;
			
			half weight1 = max(0, saturate((maxLengthAt1 - currentLength))) * FILTER_KERNEL_WEIGHTS[l];//* depth1;
			sum += sample1 * weight1;
			sampleCount += weight1;
		}
		
		half4 returnValue = sum / sampleCount;	
		returnValue.a = centerTap.a;

		return returnValue;
	}		
	
	half4 fragBlurLowSampleCount (v2f i) : COLOR 
	{
		const int TAPS = 6;
		const half FILTER_KERNEL_WEIGHTS[6] = {1.0, 0.8, 0.6, 0.375, 0.135, 0.075}; 
		
		half fgOverlap = saturate(tex2D(_FgOverlap, i.uv1.xy).a*3); // CheckForegroundOverlap(i.uv.xy);
		
		
		half4 centerTap = tex2D(_MainTex, i.uv1.xy);
		half4 sum = centerTap;// * FILTER_KERNEL_WEIGHTS[0];
		
		half2 offset = AxisFromSample(sum, fgOverlap); //GetFirstAxis(TexCoord);
		half amount = length(offset.xy);				
								
	//	half centerDepth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv.xy).x);
				
		half sampleCount = FILTER_KERNEL_WEIGHTS[0];
		half4 steps = (offset.xy * _MainTex_TexelSize.xy).xyxy;
		steps.zw *= -1; 
		for(int l=1; l < TAPS; l++)
		{
			//djhkjsdhkjashdkjashdkj kashd  as iiiiii
			half4 sampleUV = i.uv1.xyxy + steps * l;
			
			// Color samples
			half4 sample0 = tex2D(_MainTex, sampleUV.xy);
			half4 sample1 = tex2D(_MainTex, sampleUV.zw);
			
			// Maximum extent of the blur at these samples
			half maxLengthAt0;
			half maxLengthAt1;
			
			maxLengthAt0 = length(AxisFromSample(sample0, fgOverlap).xy) * (TAPS+1);//length(GetFirstAxis(sampleUV.xy)) * (NUM_STEPS+1);
			maxLengthAt1 = length(AxisFromSample(sample1, fgOverlap).xy) * (TAPS+1);//length(GetFirstAxis(sampleUV.zw)) * (NUM_STEPS+1);

	//	half depth0 = Linear01Depth(tex2D(_CameraDepthTexture, sampleUV.xy).x);
		//half depth1 = Linear01Depth(tex2D(_CameraDepthTexture, sampleUV.zw).x);
		
		//depth0 = saturate(1-(centerDepth-depth0)*(centerDepth-depth0)*10.5);
		//depth1 = saturate(1-(centerDepth-depth1)*(centerDepth-depth1)*10.5);

			// Y U NO WORKY ?
			half currentLength = amount * ((half)l);
			
			half weight0 = max(0, saturate((maxLengthAt0 - currentLength))) * FILTER_KERNEL_WEIGHTS[l];//* depth0;
			sum += sample0 * weight0; 
			sampleCount += weight0;
			
			half weight1 = max(0, saturate((maxLengthAt1 - currentLength))) * FILTER_KERNEL_WEIGHTS[l];//* depth1;
			sum += sample1 * weight1;
			sampleCount += weight1;
		}
		
		half4 returnValue = sum / sampleCount;	
		returnValue.a = centerTap.a;

		return returnValue;
	}	
	
	half4 fragBlurForFgCoc (v2fBlur i) : COLOR 
	{
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy)*4;
		half4 sampleB = tex2D(_MainTex, i.uv01.xy)*2;
		half4 sampleC = tex2D(_MainTex, i.uv01.zw)*2;
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy)*0.55;
		half4 sampleG = tex2D(_MainTex, i.uv45.zw)*0.55;
		half4 sampleH = tex2D(_MainTex, i.uv67.xy)*0.2;
		half4 sampleI = tex2D(_MainTex, i.uv67.zw)*0.2;
								
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		blurredColor += sampleF; 
		blurredColor += sampleG; 
		blurredColor += sampleH; 
		blurredColor += sampleI; 
		
		blurredColor /= 11.5;
		
		//half4 alphas = half4(sampleD.a, sampleE.a, sampleH.a*5, sampleI.a*5);
		//half4 alphas2 = half4(sampleE.a, sampleF.a, sampleG.a, 1.0);
		
		//half overlapFactor = saturate(length(alphas-sampleA.aaaa/4)-0.5);

		//half4 maxedColor = max(sampleA, sampleB);
		//maxedColor = max(maxedColor, sampleC);
		
		//blurredColor.a = saturate(blurredColor.a * 4.0f);
		// to do ot not to do
		
		//blurredColor.a += overlapFactor ; // max(maxedColor.a, blurredColor.a);
		
		float4 originalCoc = tex2D(_FgOverlap,i.uv.xy).aaaa;
		originalCoc.a = saturate(originalCoc.a + 1.8*saturate(blurredColor.a-originalCoc.a));
		
		return max(blurredColor.aaaa, originalCoc.aaaa);
	}	

	half4 frag4TapBlurForLRSpawn (v2f i) : COLOR 
	{  
		half4 tapA =  tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy); 
		half4 tapB =  tex2D(_MainTex, i.uv.xy - _MainTex_TexelSize.xy);
		half4 tapC =  tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * half2(1,-1));
		half4 tapD =  tex2D(_MainTex, i.uv.xy - _MainTex_TexelSize.xy * half2(1,-1));

		half4 color = (tapA + tapB * tapB.a + tapC * tapC.a + tapD * tapD.a) / (1.0 + tapB.a + tapC.a + tapD.a);
		color.a = tapA.a;
		return color;
	}	
	
	half4 fragApplyDebug (v2f i) : COLOR 
	{		
		float4 tapHigh = tex2D (_MainTex, i.uv1.xy);		
		float4 outColor = lerp(tapHigh, half4(0,1,0,1), tapHigh.a);
		return outColor;
	}		
		
	float4 fragCaptureCoc (v2f i) : COLOR 
	{	
		float4 color = half4(0,0,0,0); //tex2D (_MainTex, i.uv1.xy);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * abs(d - _CurveParams.w) / (d + 0.0001); 
		color.a = clamp( max(0.0, color.a - _CurveParams.y), 0.0, 1.0);
		
		return color;
		
		/*
		float4 color = half4(0,0,0,0); //tex2D (_MainTex, i.uv1.xy);
		//float4 lowTap = tex2D(_TapLowA, i.uv1.xy);
		
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		
		float focalDistance01 = _CurveParams.w + _CurveParams.z;
		
		//if (d > focalDistance01) 
		color.a = (d - focalDistance01);
	 
		half coc = saturate(color.a * _CurveParams.y);
		coc += saturate(-color.a * _CurveParams.x);
		
		// we are mixing the newly calculated BG COC with the foreground COC
		// also, for foreground COC, let's scale the COC a bit to get nicer overlaps	
		//color.a = max(lowTap.a, color.a);
		
		//color.a = saturate(color.a);// + COC_SMALL_VALUE);
		//color.rgb *= color.a;
		
		color.a = coc;
		
		return saturate(color);
		*/
	} 
	
	half4 fragCaptureForegroundCoc (v2f i) : COLOR 
	{	
		float4 color = half4(0,0,0,0); //tex2D (_MainTex, i.uv1.xy);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * (_CurveParams.w-d) / (d + 0.0001);
		color.a = clamp(max(0.0, color.a - _CurveParams.y), 0.0, 1.0);
		
		return color;	
			
		/*
		half4 color = tex2D (_MainTex, i.uv.xy);
		color.a = 0.0;

		//#if SHADER_API_D3D9
		//if (_MainTex_TexelSize.y < 0)
		//	i.uv1.xy = i.uv1.xy * half2(1,-1)+half2(0,1);
		//#endif

		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);	
		
		float focalDistance01 = (_CurveParams.w - _CurveParams.z);	
		
		if (d < focalDistance01) 
			color.a = (focalDistance01 - d);
		
		color.a = saturate(color.a * _CurveParams.x);	
				
		return color;	
		*/
	}	
	
	half4 fragCopyAlpha4Tap (v2f i) : COLOR {	
		half4 tapA =  tex2D(_MainTex, i.uv.xy + 0.5*_MainTex_TexelSize.xy);
		half4 tapB =  tex2D(_MainTex, i.uv.xy - 0.5*_MainTex_TexelSize.xy);
		half4 tapC =  tex2D(_MainTex, i.uv.xy + 0.5*_MainTex_TexelSize.xy * half2(1,-1));
		half4 tapD =  tex2D(_MainTex, i.uv.xy - 0.5*_MainTex_TexelSize.xy * half2(1,-1));	
		return (tapA+tapB+tapC+tapD)/4;
	}
	
	half4 fragPrepare (v2f i) : COLOR {	
		half4 from = tex2D(_MainTex, i.uv1.xy);
		half square = from.a * from.a;
		square*=square;
		from.a = saturate(square*square);
		//from.rgb = 0;//half3(0,0,1); // debug
		return from;
	}
		
	ENDCG
	
Subshader {
 
 // pass 0
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureCoc
      
      ENDCG
  	}

 // pass 1
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragApplyDebug

      ENDCG
  	}

 // pass 2

 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragBlurForFgCoc

      ENDCG
  	}
  	
  	
 // pass 3
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
	  ColorMask A
	  BlendOp Max, Max
	  Blend One One, One One

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureCoc

      ENDCG
  	}  
  	 	

 // pass 4
  
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
	  ColorMask A

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureForegroundCoc

      ENDCG
  	} 

 // pass 5
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurHighSampleCount

      ENDCG
  	} 

 // pass 6
 
 Pass { 
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag4TapBlurForLRSpawn

      ENDCG
  	} 

 // pass 7
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
  	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCopyAlpha4Tap

      ENDCG
  	} 
  	
 // pass 8
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Blend SrcAlpha OneMinusSrcAlpha
	  Fog { Mode off }       

      CGPROGRAM
      #pragma glsl
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragPrepare
      ENDCG
  	}   
  	

 // pass 9
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurLowSampleCount

      ENDCG
  	}   	 	 	  	 	 	  	

 // pass 10
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurPoisson

      ENDCG
  	}   

 // pass 11
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurProduction

      ENDCG
  	} 

 // pass 12
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurProductionLowRez

      ENDCG
  	} 
  	
  	// pass 13
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurPoissonLowRez

      ENDCG
  	}  
  	
 // pass 14
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment fragBlurMovie

      ENDCG
  	} 

 // pass 15
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurMovieLowRez

      ENDCG
  	}   			
}
  
Fallback off

}