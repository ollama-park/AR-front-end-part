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
    }
    
	public void Request()
	{
		string requestText = writer.GetRequest();
		LlmRequestDTOs request = new LlmRequestDTOs
		{
			userRequest = requestText
		};
		string json = JsonUtility.ToJson(request);
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
