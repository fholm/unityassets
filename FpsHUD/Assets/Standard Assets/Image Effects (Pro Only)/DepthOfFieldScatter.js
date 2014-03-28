
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Depth of Field (HDR, Scatter, Lens Blur)") 

class DepthOfFieldScatter extends PostEffectsBase {	
    public var visualizeFocus : boolean = false;
	
	public var focalLength : float = 10.0f;
	public var focalSize : float = 0.05f; 
	public var aperture : float = 10.0f;

	public var focalTransform : Transform = null;

	public var maxBlurSize : float = 2.0f; 
	
	public enum BlurQuality {
		Low = 0,
		Medium = 1,
		High = 2,
	}
	
	public enum BlurResolution {
		High = 0,
		Low = 1,
	}
	 
	public var blurQuality : BlurQuality = BlurQuality.Medium;
	public var blurResolution : BlurResolution = BlurResolution.Low;
	
    public var foregroundBlur : boolean = false;	
	public var foregroundOverlap : float = 0.55f;
	
	public var dofHdrShader : Shader;		
	
	private var focalDistance01 : float = 10.0f;	
	private var dofHdrMaterial : Material = null;		        
        	
	function CheckResources () : boolean {		
		CheckSupport (true);
	
		dofHdrMaterial = CheckShaderAndCreateMaterial (dofHdrShader, dofHdrMaterial); 
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		  
	}
	
	function FocalDistance01 (worldDist : float) : float {
		return camera.WorldToViewportPoint((worldDist-camera.nearClipPlane) * camera.transform.forward + camera.transform.position).z / (camera.farClipPlane-camera.nearClipPlane);	
	}
			
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
		if(CheckResources () == false) {
			Graphics.Blit (source, destination);
			return;
		}
		
		var i : int = 0;
		var internalBlurWidth : float = maxBlurSize;
		var blurRtDivider : int = blurResolution == BlurResolution.High ? 1 : 2;
		
		// clamp values so they make sense

		if (aperture < 0.0f) aperture = 0.0f;
		if (maxBlurSize < 0.0f) maxBlurSize = 0.0f;
		focalSize = Mathf.Clamp(focalSize, 0.0f, 0.3f);
					
		// focal & coc calculations

		focalDistance01 = focalTransform ? (camera.WorldToViewportPoint (focalTransform.position)).z / (camera.farClipPlane) : FocalDistance01 (focalLength);
		
		var isInHdr : boolean = source.format == RenderTextureFormat.ARGBHalf;
		
		var scene : RenderTexture = blurRtDivider > 1 ? RenderTexture.GetTemporary (source.width/blurRtDivider, source.height/blurRtDivider, 0, source.format) : null;			
		if (scene) scene.filterMode = FilterMode.Bilinear;
		var rtLow : RenderTexture = RenderTexture.GetTemporary (source.width/(2*blurRtDivider), source.height/(2*blurRtDivider), 0, source.format);		
		var rtLow2 : RenderTexture = RenderTexture.GetTemporary (source.width/(2*blurRtDivider), source.height/(2*blurRtDivider), 0, source.format);			
		if (rtLow) rtLow.filterMode = FilterMode.Bilinear;
		if (rtLow2) rtLow2.filterMode = FilterMode.Bilinear;
	
		dofHdrMaterial.SetVector ("_CurveParams", Vector4 (0.0f, focalSize, aperture/10.0f, focalDistance01));
		
		// foreground blur
		
		if (foregroundBlur) {			
			var rtLowTmp : RenderTexture = RenderTexture.GetTemporary (source.width/(2*blurRtDivider), source.height/(2*blurRtDivider), 0, source.format);		
		
			// Capture foreground CoC only in alpha channel and increase CoC radius
			Graphics.Blit (source, rtLow2, dofHdrMaterial, 4); 
			dofHdrMaterial.SetTexture("_FgOverlap", rtLow2); 
			
			var fgAdjustment : float = internalBlurWidth * foregroundOverlap * 0.225f; 
			dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, fgAdjustment , 0.0f, fgAdjustment));
			Graphics.Blit (rtLow2, rtLowTmp, dofHdrMaterial, 2);
			dofHdrMaterial.SetVector ("_Offsets", Vector4 (fgAdjustment, 0.0f, 0.0f, fgAdjustment));		
			Graphics.Blit (rtLowTmp, rtLow, dofHdrMaterial, 2);	 			
			
			dofHdrMaterial.SetTexture("_FgOverlap", null); // NEW: not needed anymore
			// apply adjust FG coc back to high rez coc texture
			Graphics.Blit(rtLow, source, dofHdrMaterial, 7);	
			
			RenderTexture.ReleaseTemporary(rtLowTmp);					
		}
		else 
			dofHdrMaterial.SetTexture("_FgOverlap", null); // ugly FG overlaps as a result
		
		// capture remaing CoC (fore & *background*)
		
		Graphics.Blit (source, source, dofHdrMaterial, foregroundBlur ? 3 : 0);		
		
		var cocRt : RenderTexture = source;
		
		if(blurRtDivider>1) {
			Graphics.Blit (source, scene, dofHdrMaterial, 6);		
			cocRt = scene;	
		}
		
		// spawn a few low rez parts in high rez image to get a bigger blur
		// resulting quality is higher than directly blending preblurred buffers
		
		Graphics.Blit(cocRt, rtLow2, dofHdrMaterial, 6); 
		Graphics.Blit(rtLow2, cocRt, dofHdrMaterial, 8);
		
		//  blur and apply to color buffer 
		
		var blurPassNumber : int = 10;
		switch(blurQuality) {
			case BlurQuality.Low:
				blurPassNumber = blurRtDivider > 1 ? 13 : 10;
				break;
			case BlurQuality.Medium:
				blurPassNumber = blurRtDivider > 1 ? 12 : 11;
				break;
			case BlurQuality.High:
				blurPassNumber = blurRtDivider > 1 ? 15 : 14;
				break;				
			default:
				Debug.Log("DOF couldn't find valid blur quality setting", transform);
				break;
		}
		
		if(visualizeFocus) {
			Graphics.Blit (source, destination, dofHdrMaterial, 1);
		}
		else { 		 
			dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, 0.0f , 0.0f, internalBlurWidth));
			dofHdrMaterial.SetTexture("_LowRez", cocRt); // only needed in low resolution profile. and then, ugh, we get an ugly transition from nonblur -> blur areas
			Graphics.Blit (source, destination, dofHdrMaterial, blurPassNumber);	 
		}
		
		if(rtLow) RenderTexture.ReleaseTemporary(rtLow);
		if(rtLow2) RenderTexture.ReleaseTemporary(rtLow2);		
		if(scene) RenderTexture.ReleaseTemporary(scene); 
	}	
}
