using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using System.Collections.Generic;

public class Talker : MonoBehaviour
{
	public TTSSpeaker tts;

	private Queue<string> speechQueue = new Queue<string>();
	public bool isSpeaking = false;

	void Start()
	{
		if (tts == null)
		{
			tts = FindObjectOfType<TTSSpeaker>();
		}
			

		//tts.OnPlaybackComplete += OnSpeakComplete;
	}

	public void Say(string text)
	{
		if (string.IsNullOrEmpty(text))
			return;

		speechQueue.Enqueue(text);

		if (!isSpeaking)
			SpeakNext();
	}

	private void SpeakNext()
	{
		if (speechQueue.Count == 0)
		{
			isSpeaking = false;
			return;
		}

		isSpeaking = true;
		string next = speechQueue.Dequeue();
		tts.Speak(next);
	}

	private void OnSpeakComplete(TTSSpeaker speaker, string text)
	{
		SpeakNext();
	}
}
