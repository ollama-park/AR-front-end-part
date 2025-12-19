using UnityEngine;
using Unity.InferenceEngine;
//using Unity.InferenceEngine.Tensor;
using System.Collections;
public class YoloModelControl : MonoBehaviour
{
	public ModelAsset modelAsset;
	private Model model;
	private Worker worker;
	void Start()
	{
		model = ModelLoader.Load(modelAsset);

		StartCoroutine(WarmUp());
	}

	IEnumerator WarmUp()
	{
		FunctionalGraph graph = new FunctionalGraph();
		FunctionalTensor[] inputs = graph.AddInputs(model);
		FunctionalTensor[] outputs = Functional.Forward(model, inputs);
		
		Model runtimeModel = graph.Compile(outputs);
		worker = new Worker(runtimeModel, BackendType.GPUCompute);

		var input = new Tensor<float>(new TensorShape(1, 3, 640, 640));
		worker.Schedule(input);

		input.Dispose();

		yield return null;
		Debug.Log("Model warmed up and ready");
	}
	
	public float[] GetOutput(Texture2D picture)
	{
		Tensor<float> input = TextureConverter.ToTensor(picture, 640, 640, 1);
		worker.Schedule(input);
		
		Tensor<float> output = worker.PeekOutput() as Tensor<float>;
		
		
		return output.DownloadToArray();;
	}

	void OnDestroy()
	{
		worker?.Dispose();
	}
}
