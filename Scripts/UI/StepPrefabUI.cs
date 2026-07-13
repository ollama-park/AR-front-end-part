using UnityEngine;
using TMPro; 

public class StepPrefabUI : MonoBehaviour
{
	public TMP_Text stepNumberText;
	public TMP_Text stepText;

	public void Setup(int number, string text)
	{
		stepNumberText.text = number.ToString();
		stepText.text = text;
	}
}