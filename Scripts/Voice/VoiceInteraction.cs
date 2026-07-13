using UnityEngine;
using System.Collections;

public class VoiceInteraction : MonoBehaviour
{
	[Header("Scripts")]
	public Writer writer;
	public API Api;
	public Talker talker;
	[Header("Endpoints")]
	[SerializeField] private string helpEndpoint = "help";
	
	[Header("Texts")]
	public TMPro.TMP_Text responseText;
	
	[Header("Other")]
	public int MaxTTSLength;
	public float waitBeforeSpeak = 1.0f;
	
	public OVRHand leftHand;
	private bool wasPinching;

	
    void Start()
    {
	    if(writer == null)
	    {
	    	writer = FindObjectOfType<Writer>();
	    }
	    if(talker == null)
	    {
	    	talker = FindObjectOfType<Talker>();
	    }
	    if(Api == null)
	    {
	    	Api = FindObjectOfType<API>();
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    if(OVRInput.GetDown(OVRInput.Button.Four))
	    {
	    	writer.StartListen();
	    }
	    CheckPinch(leftHand);
    }
    
	void CheckPinch(OVRHand hand)
	{
		if (hand == null || !hand.IsTracked)
			return;

		bool isPinching = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.8f;

		if (isPinching && !wasPinching)
		{
			writer.StartListen();
		}

		wasPinching = isPinching;
	}
    
	public void Request()
	{
		string requestText = writer.GetRequest();
		Debug.Log("UNITY SALIH: Request Started");
		LlmRequestDTOs request = new LlmRequestDTOs
		{
			request = requestText
		};
		string json = JsonUtility.ToJson(request);
		Debug.Log("UNITY SALIH: json request is:" + json);
		Api.LlmPostMethod(helpEndpoint, json);
	}
	
	public void PlayResponse(string response)
	{
		responseText.text = response;
		StopAllCoroutines();
		StartCoroutine(SpeakInChunks(response));
	}

	private IEnumerator SpeakInChunks(string text)
	{
		int index = 0;

		while (index < text.Length)
		{
			int length = Mathf.Min(MaxTTSLength, text.Length - index);
			string chunk = text.Substring(index, length);

			talker.Say(chunk);

			yield return new WaitUntil(() => !talker.isSpeaking);

			index += length;
		}
	}

}
