using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ContextObject
{
	public string object_name;
	public float positionX;
	public float positionY;
	public float positionZ;
}

[System.Serializable]
public class LlmRequestDTOs
{
	public List<ContextObject> context;
	public string request;
}