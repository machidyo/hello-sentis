using System.Linq;
using UnityEngine;
using Unity.Sentis;

public class StyleChange : MonoBehaviour
{
    public const int IMAGE_SIZE = 224; // Modelの仕様

    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private RenderTexture outputTexture;

    private WebCamera webCamera;

    private Model runtimeModel;
    private IWorker worker;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);

        webCamera = FindAnyObjectByType<WebCamera>();
    }

    void Update()
    {
        var inputTensor = TextureConverter.ToTensor(webCamera.Texture, IMAGE_SIZE, IMAGE_SIZE, 3);
        Inference(inputTensor);
        inputTensor.Dispose();
    }
    
    void OnDestroy()
    {
        worker.Dispose();
    }

    private void Inference(Tensor input)
    {
        worker.Execute(input);
        if (worker.PeekOutput() is not TensorFloat output) { return; /* ガード */ }
        
        // MakeReadableしないと以下のエラーが出る
        // InvalidOperationException: Tensor data cannot be read from, use .MakeReadable() to allow reading from tensor.
        output.MakeReadable();
        
        // 正規化しないと正常な結果が得られない
        var normalized = output.ToReadOnlyArray().Select(x => x / 255f).ToArray();
        var normalizedTensor = new TensorFloat(output.shape, normalized);
        
        TextureConverter.RenderToTexture(normalizedTensor, outputTexture);
        
        // 各STEP毎にTensorを破棄しないとメモリリークの恐れあり
        output.Dispose(); 
        normalizedTensor.Dispose();
    }
}