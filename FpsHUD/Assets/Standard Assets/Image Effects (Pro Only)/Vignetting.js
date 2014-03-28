
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Vignette and Chromatic Aberration")

class Vignetting extends PostEffectsBase {
	
	public var intensity : float = 0.375f;
	public var chromaticAberration : float = 0.2f;
	public var blur : float = 0.1f;
	public var blurSpread : float = 1.5f;
	
    // needed shaders & materials
	
	public var vignetteShader : Shader;
	private var vignetteMaterial : Material;
	
	public var separableBlurShader : Shader;
	private var separableBlurMaterial : Material;	
	
	public var chromAberrationShader : Shader;
	private var chromAberrationMaterial : Material;
	
	function OnDisable()
	{
		if (vignetteMaterial)
		    DestroyImmediate(vignetteMaterial);
		if (separableBlurMaterial)
		    DestroyImmediate(separableBlurMaterial);
		if (chromAberrationMaterial)
		    DestroyImmediate(chromAberrationMaterial);
	}
	
	function CheckResources () : boolean {	
		CheckSupport (false);	
	
		vignetteMaterial = CheckShaderAndCreateMaterial (vignetteShader, vignetteMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial (separableBlurShader, separableBlurMaterial);
		chromAberrationMaterial = CheckShaderAndCreateMaterial (chromAberrationShader, chromAberrationMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}	
				
		var widthOverHeight : float = (1.0f * source.width) / (1.0f * source.height);
		var oneOverBaseSize : float = 1.0f / 512.0f;				
		
		var color : RenderTexture = RenderTexture.GetTemporary (source.width, source.height, 0);	
		var halfRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 2.0, source.height / 2.0, 0);		
		var quarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4.0, source.height / 4.0, 0);	
		var secondQuarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4.0, source.height / 4.0, 0);	
				
		Graphics.Blit (source, halfRezColor, chromAberrationMaterial, 0);
		Graphics.Blit (halfRezColor, quarterRezColor);	
				
		for (var it : int = 0; it < 2; it++ ) {
			separableBlurMaterial.SetVector ("offsets", Vector4 (0.0, blurSpread * oneOverBaseSize, 0.0, 0.0));	
			Graphics.Blit (quarterRezColor, secondQuarterRezColor, separableBlurMaterial); 
			separableBlurMaterial.SetVector ("offsets", Vector4 (blurSpread * oneOverBaseSize / widthOverHeight, 0.0, 0.0, 0.0));	
			Graphics.Blit (secondQuarterRezColor, quarterRezColor, separableBlurMaterial);		
		}		
		
		vignetteMaterial.SetFloat ("_Intensity", intensity);
		vignetteMaterial.SetFloat ("_Blur", blur);
		vignetteMaterial.SetTexture ("_VignetteTex", quarterRezColor);
		Graphics.Blit (source, color, vignetteMaterial); 				
		
		chromAberrationMaterial.SetFloat ("_ChromaticAberration", chromaticAberration);
		Graphics.Blit (color, destination, chromAberrationMaterial, 1);	
		
		RenderTexture.ReleaseTemporary (color);
		RenderTexture.ReleaseTemporary (halfRezColor);			
		RenderTexture.ReleaseTemporary (quarterRezColor);	
		RenderTexture.ReleaseTemporary (secondQuarterRezColor);	
	
	}

}