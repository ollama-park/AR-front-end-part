using UnityEngine;
using System.Collections.Generic;

public class DetectedObjectsRegistry : MonoBehaviour
{
	public static DetectedObjectsRegistry Instance { get; private set; }

	private readonly List<ContextObject> m_detectedObjects = new();

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;
	}

	public void UpdateDetectedObjects(List<ContextObject> objects)
	{
		m_detectedObjects.Clear();
		m_detectedObjects.AddRange(objects);
	}

	public List<ContextObject> GetDetectedObjects() => new List<ContextObject>(m_detectedObjects);
}