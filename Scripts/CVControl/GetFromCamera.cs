using UnityEngine;
using Meta.XR;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEngine.UI;

public class GetFromCamera : MonoBehaviour
{
	private PassthroughCameraAccess cameraAccess;
	public YoloModelControl yolo;
	private Texture2D outputTexture;
	public Text testText, abobaText; 
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    cameraAccess = GetComponent<PassthroughCameraAccess>();
	    cameraAccess.RequestedResolution = new Vector2Int(640, 640);
	    
	    outputTexture = new Texture2D(640, 640, TextureFormat.RGBA32, false);
	    
	    abobaText.text = "Aboba";
    }

    void Update()
    {
	    if(OVRInput.Get(OVRInput.Button.One))
	    {
	    	GetFrame();
	    	//abobaText.text = "Baobab";
	    }
    }
    
	public void GetFrame()
	{
		abobaText.text = "Magaz";
		Texture tex = cameraAccess.GetTexture();
		
		if (tex == null)
		{
			abobaText.text = "Texture is NULL!";
			return;
		}
        
		abobaText.text = $"GetTexture: {tex.width}x{tex.height}";
		
		if (outputTexture == null)
		{
			outputTexture = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
		}
		
		if(tex is RenderTexture)
		{
			AsyncGPUReadback.Request(tex, 0, request =>
			{
				HandleReadback(request);
			});
		}
		else
		{
			AsyncGPUReadback.Request(tex, 0, TextureFormat.RGBA32, request =>
			{
				HandleReadback(request);
			});
		}
	}
	
	private void HandleReadback(AsyncGPUReadbackRequest request)
	{
		if (request.hasError)
		{
			abobaText.text = "Readback ERROR!";
			return;
		}
        
		try
		{
			abobaText.text = "Started NativeArray";
            
			NativeArray<Color32> data = request.GetData<Color32>();
            
			if (data.Length == 0)
			{
				abobaText.text = "Data is EMPTY!";
				return;
			}
            
			int width = request.width;
			int height = request.height;
            
			if (outputTexture.width != width || outputTexture.height != height)
			{
				outputTexture.Reinitialize(width, height);
			}
            
			outputTexture.SetPixels32(data.ToArray());
			outputTexture.Apply();
            
			abobaText.text = "Applied";
            
			float[] results = yolo.GetOutput(outputTexture);
			testText.text = $"Results: {results.Length}";
		}
			catch (System.Exception e)
			{
				abobaText.text = $"Exception: {e.Message}";
				Debug.LogError($"Readback exception: {e}");
			}
	}
    
	void OnDestroy()
	{
		if (outputTexture != null)
		{
			Destroy(outputTexture);
		}
	}
}
