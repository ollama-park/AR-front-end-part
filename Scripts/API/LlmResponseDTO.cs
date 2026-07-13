using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StepItem
{
	public string step;
}

[System.Serializable]
public class LlmResponseDTO
{
	public List<string> goal_targets;
	public string main_response;
	public List<StepItem> steps;
}