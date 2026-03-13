using UnityEngine;using UnityEngine.Networking;
using System.Collections;
using System.Text;


public class API : MonoBehaviour
{
	public string host = "http://10.39.129.134:8000";
	
	[Header("Codes")]
	public VoiceInteraction voiceInteraction;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		if(voiceInteraction == null)
		{
			voiceInteraction = FindObjectOfType<VoiceInteraction>();
		}
	}
	
	public void LlmPostMethod(string endpoint, string json)
	{
		StartCoroutine(Post(endpoint, json));
	}
	public void FeedbackPostMethod(string endpoint, string json)
	{
		StartCoroutine(Post(endpoint, json));
	}
	
	private IEnumerator Post(string endpoint, string json)
	{
		string url = host + "/" + endpoint;
		
		using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
		{
			byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 120;
			
			yield return request.SendWebRequest();
			
			if(request.result == UnityWebRequest.Result.Success)
			{
				LlmResponseDTO data;
				
				try 
				{
					data = JsonUtility.FromJson<LlmResponseDTO>(request.downloadHandler.text);
				}catch
				{
					Debug.LogError("Invalid JSON response from API.");
					yield break;
				}
				if (data == null || string.IsNullOrEmpty(data.llmResponse))
				{
					Debug.LogError("Empty helpResponse in server reply.");
					yield break;
				}
				string responseText = data.llmResponse;
				voiceInteraction.PlayResponse(responseText);
				
			}
			yield return null;
		}
	}
	
}
