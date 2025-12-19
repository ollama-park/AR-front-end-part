using UnityEngine;
using Unity.InferenceEngine;
using System.IO;
public class QuantizeModel : MonoBehaviour
{
	public Model model;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		string outPath =  Application.streamingAssetsPath;
		string path = Path.Combine(outPath, "yolov5s16f.onnx");
		model = ModelLoader.Load(outPath + "/yolov5s.sentis");
		QuantizeAndSave(model, path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void QuantizeAndSave(Model model, string path)
	{
		ModelQuantizer.QuantizeWeights(QuantizationType.Float16, ref model);
		ModelWriter.Save(path, model);
	}

}
