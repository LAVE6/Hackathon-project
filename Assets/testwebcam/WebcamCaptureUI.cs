
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class WebcamCaptureUI : MonoBehaviour
{
    public RawImage webcamDisplay;
    public RawImage previewDisplay;
    public Image flashImage;
    public string saveFolderPath = "";

    private WebCamTexture webcamTexture;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;
        webcamDisplay.material.mainTexture = webcamTexture;
        webcamTexture.Play();

        if (flashImage != null)
            flashImage.color = new Color(1, 1, 1, 0);
    }

    public void CaptureFrame()
    {
        if (webcamTexture.width <= 16 || webcamTexture.height <= 16)
        {
            Debug.LogWarning("Webcam not ready yet.");
            return;
        }

        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator CaptureAndSave()
    {
        if (flashImage != null)
            StartCoroutine(FlashEffect());

        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();

        byte[] bytes = photo.EncodeToPNG();

        string folderPath = saveFolderPath;
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = Path.Combine(Application.persistentDataPath, "SavedFrames");
        }

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "frame_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Saved frame to: {filePath}");

        yield return LoadPreview(filePath);
    }

    private IEnumerator LoadPreview(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        previewDisplay.texture = tex;
       // previewDisplay.SetNativeSize();

        yield return null;
    }

    private IEnumerator FlashEffect()
    {
        flashImage.color = new Color(1, 1, 1, 0.8f);
        yield return new WaitForSeconds(0.1f);
        flashImage.color = new Color(1, 1, 1, 0);
    }
}
