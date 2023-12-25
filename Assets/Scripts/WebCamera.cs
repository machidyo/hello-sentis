using UnityEngine;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    private RawImage rawImage;
    private WebCamTexture webCamTexture;

    public WebCamTexture Texture => webCamTexture;
    
    void Start ()
    {
        // Webカメラの開始
        rawImage = GetComponent<RawImage>();
        webCamTexture = new WebCamTexture(StyleChange.IMAGE_SIZE, StyleChange.IMAGE_SIZE, 30);
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }
}