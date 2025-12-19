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
	public Text testText; 
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    cameraAccess = GetComponent<PassthroughCameraAccess>();
	    cameraAccess.RequestedResolution = new Vector2Int(640, 640);
    }

    // Update is called once per frame
    void Update()
    {
	    if(OVRInput.Get(OVRInput.Button.One))
	    {
	    	GetFrame();
	    }
    }
    
	public void GetFrame()
	{
		Texture tex = cameraAccess.GetTexture();
		AsyncGPUReadback.Request(tex,0,  TextureFormat.RGBA32, request =>
		{
			if (!request.hasError)
			{
				NativeArray<Color32> data = request.GetData<Color32>();
				outputTexture.SetPixels32(data.ToArray());
				outputTexture.Apply();
				float[] results = yolo.GetOutput(outputTexture);
				testText.text = results.ToString();
			}
		});
	}
}
