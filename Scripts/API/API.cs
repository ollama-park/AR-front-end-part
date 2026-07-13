// Simple version (commented out) — updated to new DTOs
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using System.Text;
//using System.Collections.Generic;

//public class API : MonoBehaviour
//{
//    public string host = "https://ar.totatato.com";

//    [Header("Codes")]
//    public VoiceInteraction voiceInteraction;

//    protected void Start()
//    {
//        if (voiceInteraction == null)
//        {
//            voiceInteraction = FindObjectOfType<VoiceInteraction>();
//        }
//    }

//    public void LlmPostMethod(string endpoint, string json)
//    {
//        Debug.Log("UNITY SALIH: llmpost method started");
//        StartCoroutine(Post(endpoint, json));
//    }

//    public void FeedbackPostMethod(string endpoint, string json)
//    {
//        StartCoroutine(Post(endpoint, json));
//    }

//    private IEnumerator Post(string endpoint, string json)
//    {
//        string url = host + "/" + endpoint;
//        Debug.Log("UNITY SALIH: Post Coroutine started with host: " + host);

//        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
//        {
//            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
//            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//            request.downloadHandler = new DownloadHandlerBuffer();
//            request.SetRequestHeader("Content-Type", "application/json");
//            request.timeout = 120;

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                LlmResponseDTO data;

//                try
//                {
//                    data = JsonUtility.FromJson<LlmResponseDTO>(request.downloadHandler.text);
//                }
//                catch
//                {
//                    Debug.LogError("UNITY SALIH: Invalid JSON response from API.");
//                    yield break;
//                }

//                if (data == null || string.IsNullOrEmpty(data.main_response))
//                {
//                    Debug.LogError("UNITY SALIH: Empty main_response in server reply.");
//                    yield break;
//                }

//                Debug.Log("UNITY SALIH: main_response: " + data.main_response);

//                if (data.goal_targets != null)
//                    Debug.Log("UNITY SALIH: goal_targets: " + string.Join(", ", data.goal_targets));

//                if (data.steps != null)
//                    for (int i = 0; i < data.steps.Count; i++)
//                        Debug.Log($"UNITY SALIH: step[{i}]: " + data.steps[i].step);

//                voiceInteraction.PlayResponse(data.main_response);
//            }

//            yield return null;
//        }
//    }
//}

// Delay-measuring version (active)
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using PassthroughCameraSamples.MultiObjectDetection;

public class API : MonoBehaviour
{
	public string host = "https://ar.totatato.com";

	[Header("Codes")]
	public VoiceInteraction voiceInteraction;
	public SentisInferenceUiManager sentisUiManager;
	public ShowSteps showSteps;
	private StreamWriter logWriter;
	private object logObj = new object();
	private string logPath;
	

	protected void Start()
	{
		if (voiceInteraction == null)
		{
			voiceInteraction = FindObjectOfType<VoiceInteraction>();
		}
		if(showSteps == null)
		{
			showSteps = FindObjectOfType<ShowSteps>();
		}
		if (sentisUiManager == null)
		{
			sentisUiManager = FindObjectOfType<SentisInferenceUiManager>();
		}
			

		logPath = Path.Combine(Application.persistentDataPath,
			$"stream_log_{gameObject.name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
		try
		{
			logWriter = new StreamWriter(logPath, append: false);
			logWriter.WriteLine("timestamp,delay");
			logWriter.Flush();
			Debug.Log($"UNITY SALIH: {gameObject.name}: Logging to {logPath}");
		}
			catch (Exception e)
			{
				Debug.LogError($"UNITY SALIH: {gameObject.name}: Failed to open log: {e.Message}");
			}
	}

	public void LlmPostMethod(string endpoint, string userRequest)
	{
		Debug.Log("UNITY SALIH: llmpost method started");

		var context = DetectedObjectsRegistry.Instance != null
			? DetectedObjectsRegistry.Instance.GetDetectedObjects()
			: new List<ContextObject>();

		var requestDTO = new LlmRequestDTOs
		{
			context = context,
			request = userRequest
		};

		string json = JsonUtility.ToJson(requestDTO);
		Debug.Log("UNITY SALIH: Sending JSON: " + json);

		StartCoroutine(Post(endpoint, json));
	}

	public void FeedbackPostMethod(string endpoint, string json)
	{
		StartCoroutine(Post(endpoint, json));
	}

	private IEnumerator Post(string endpoint, string json)
	{
		string url = host + "/" + endpoint;
		Debug.Log("UNITY SALIH: Post Coroutine started with host: " + host);

		float startTime = Time.realtimeSinceStartup;

		using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
		{
			byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 500;

			yield return request.SendWebRequest();

			float endTime = Time.realtimeSinceStartup;
			float delay = endTime - startTime;

			if (logWriter != null)
			{
				lock (logObj)
				{
					logWriter.WriteLine($"{DateTime.UtcNow:O},{delay}");
					logWriter.Flush();
				}
			}

			Debug.Log($"UNITY SALIH: Delay = {delay} seconds");

			if (request.result == UnityWebRequest.Result.Success)
			{
				LlmResponseDTO data;

				try
				{
					data = JsonUtility.FromJson<LlmResponseDTO>(request.downloadHandler.text);
				}
					catch
					{
						Debug.LogError("UNITY SALIH: Invalid JSON response from API.");
						yield break;
					}

				if (data == null || string.IsNullOrEmpty(data.main_response))
				{
					Debug.LogError("UNITY SALIH: Empty main_response in server reply.");
					yield break;
				}

				Debug.Log("UNITY SALIH: main_response: " + data.main_response);

				if (data.goal_targets != null)
				{
					Debug.Log("UNITY SALIH: goal_targets: " + string.Join(", ", data.goal_targets));
					if (sentisUiManager != null)
					{
						sentisUiManager.FilterToGoalTargets(data.goal_targets);
					}
						
					else
					{
						Debug.LogWarning("UNITY SALIH: sentisUiManager is null, cannot filter boxes.");
					}
						
				}
					

				if (data.steps != null)
				{
					for (int i = 0; i < data.steps.Count; i++)
					{
						Debug.Log($"UNITY SALIH: step[{i}]: " + data.steps[i].step);
					}
					
					showSteps.SetSteps(data.steps);
						
				}
					

				voiceInteraction.PlayResponse(data.main_response);
			}
		}
	}
}