
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Noise And Grain (Overlay)")

class NoiseAndGrain extends PostEffectsBase {

	public var strength : float = 1.0f;
	public var blackIntensity : float = 1.0f;
	public var whiteIntensity : float = 1.0f;
	
	public var redChannelNoise : float = 0.975f;
	public var greenChannelNoise : float = 0.875f;
	public var blueChannelNoise : float = 1.2f;
	
	public var redChannelTiling : float = 24.0f;
	public var greenChannelTiling : float = 28.0f;
	public var blueChannelTiling : float = 34.0f;
	
	public var filterMode : FilterMode = FilterMode.Bilinear;
			
	public var noiseShader : Shader;
	public var noiseTexture : Texture2D;
	
	private var noiseMaterial : Material = null;
	
	function OnDisable()
	{
	    if (noiseMaterial)
	        DestroyImmediate(noiseMaterial);
	}
	
	function CheckResources () : boolean {
		CheckSupport (false);
		
		noiseMaterial = CheckShaderAndCreateMaterial (noiseShader, noiseMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
							
		noiseMaterial.SetVector ("_NoisePerChannel", Vector3(redChannelNoise, greenChannelNoise, blueChannelNoise));
		noiseMaterial.SetVector ("_NoiseTilingPerChannel", Vector3(redChannelTiling, greenChannelTiling, blueChannelTiling));
		noiseMaterial.SetVector ("_NoiseAmount", Vector3(strength, blackIntensity, whiteIntensity));
		noiseMaterial.SetTexture ("_NoiseTex", noiseTexture);
	   	noiseTexture.filterMode = filterMode; 

		DrawNoiseQuadGrid (source, destination, noiseMaterial, noiseTexture, 0);
	}
		
	static function DrawNoiseQuadGrid (source : RenderTexture, dest : RenderTexture, fxMaterial : Material, noise : Texture2D, passNr : int) {
		RenderTexture.active = dest;
		
		var noiseSize : float = (noise.width * 1.0f);
		
		var tileSize : float = noiseSize;
		
		var subDs : float = (1.0f * source.width) / tileSize;
	       
		fxMaterial.SetTexture ("_MainTex", source);	        
	                
		GL.PushMatrix ();
		GL.LoadOrtho ();	
			
		var aspectCorrection : float = (1.0f * source.width) / (1.0f * source.height);
		var stepSizeX : float = 1.0f / subDs;
		var stepSizeY : float = stepSizeX * aspectCorrection; 
	   	var texTile : float = tileSize / (noise.width * 1.0f);
	   		    	    	
		fxMaterial.SetPass (passNr);	
		
	    GL.Begin (GL.QUADS);
	    
	   	for (var x1 : float = 0.0; x1 < 1.0; x1 += stepSizeX) {
	   		for (var y1 : float = 0.0; y1 < 1.0; y1 += stepSizeY) { 
	   			
	   			var tcXStart : float = Random.Range (0.0f, 1.0f);
	   			var tcYStart : float = Random.Range (0.0f, 1.0f);
	   			
	   			tcXStart = Mathf.Floor(tcXStart*noiseSize) / noiseSize;
	   			tcYStart = Mathf.Floor(tcYStart*noiseSize) / noiseSize;
	   			
	   			//var texTileMod : float = Mathf.Sign (Random.Range (-1.0f, 1.0f));
	   			var texTileMod : float = 1.0f / noiseSize;
							
			    GL.MultiTexCoord2 (0, tcXStart, tcYStart); 
			    GL.MultiTexCoord2 (1, 0.0f, 0.0f); 
			    GL.Vertex3 (x1, y1, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart); 
			    GL.MultiTexCoord2 (1, 1.0f, 0.0f); 
			    GL.Vertex3 (x1 + stepSizeX, y1, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart + texTile * texTileMod); 
			    GL.MultiTexCoord2 (1, 1.0f, 1.0f); 
			    GL.Vertex3 (x1 + stepSizeX, y1 + stepSizeY, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart, tcYStart + texTile * texTileMod); 
			    GL.MultiTexCoord2 (1, 0.0f, 1.0f); 
			    GL.Vertex3 (x1, y1 + stepSizeY, 0.1);
	   		}
	   	}
	    	
		GL.End ();
	    GL.PopMatrix ();
	}
}