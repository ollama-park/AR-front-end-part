using UnityEngine;
using System.Collections.Generic;

public class ShowSteps : MonoBehaviour
{
	public GameObject stepPrefab;
	public Transform stepsHolder;
	public float offset;

	public void SetSteps(List<StepItem> steps)
	{
		CleanSteps();

		for (int i = 0; i < steps.Count; i++)
		{
			Vector3 spawnPosition = stepsHolder.localPosition + new Vector3(i * offset, 0f, 0f);
			GameObject stepGO = Instantiate(stepPrefab, spawnPosition, Quaternion.identity, stepsHolder);

			StepPrefabUI ui = stepGO.GetComponent<StepPrefabUI>();
			if (ui != null)
			{
				ui.Setup(i + 1, steps[i].step);
			}
			else
			{
				Debug.LogError($"UNITY SALIH: StepPrefab is missing StepPrefabUI component on step {i + 1}");
			}
		}
	}

	public void CleanSteps()
	{
		foreach (Transform child in stepsHolder)
		{
			Destroy(child.gameObject);
		}
	}
}