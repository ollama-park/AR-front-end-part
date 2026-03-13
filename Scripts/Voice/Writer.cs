using UnityEngine;
using Oculus.Voice.Dictation;
using TMPro;

public class Writer : MonoBehaviour
{
	public AppDictationExperience dictation;
	[SerializeField] private TMP_Text requestUIText;
	private string request = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    if(dictation == null)
	    {
	    	dictation = FindObjectOfType<AppDictationExperience>();
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void StartListen()
	{
		dictation.Activate();
		request = "";
		requestUIText.text = "";
	}
	
	public void SetRequestText(string text)
	{
		request += text;
		requestUIText.text = text;
	}
	
	public string GetRequest()
	{
		return request;
	}
	
	public void StopListen()
	{
		dictation.Deactivate();
		
	}
    
}
