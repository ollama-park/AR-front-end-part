using UnityEngine;
using TMPro;
using Meta.WitAi;
using Meta.WitAi.Events;
using Oculus.Voice;

public class GetVoice : MonoBehaviour
{
	public AppVoiceExperience voice;
	public TextMeshProUGUI text;
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		Invoke(nameof(StartListening), 0.2f);
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
		{
			StartListening();
		}
	}
	
	public void OnEnable()
	{
		voice.VoiceEvents.OnPartialTranscription.AddListener(OnPartial);
		voice.VoiceEvents.OnFullTranscription.AddListener(OnFull);
	}

	public void OnDisable()
	{
		voice.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartial);
		voice.VoiceEvents.OnFullTranscription.RemoveListener(OnFull);
	}

	public void OnPartial(string t)
	{
		text.text = t;
	}

	public void OnFull(string t)
	{
		text.text = t;
	}

	public void StartListening()
	{
		if (!voice.Active)
		{
			voice.Activate();
		}
	}

	public void StopListening()
	{
		voice.Deactivate();
	}
}
