using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScenes : MonoBehaviour
{
	public void OpenBrightness()
	{
		SceneManager.LoadScene("BrightnessEstimation");	
	}
	public void OpenCameraToWorld()
	{
		SceneManager.LoadScene("CameraToWorld");	
	}
	public void OpenCameraView()
	{
		SceneManager.LoadScene("CameraViewer");	
	}
	public void OpenMOD()
	{
		SceneManager.LoadScene("MultiObjectDetection");	
	}
}
