using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class PhotoBooth : MonoBehaviour
{
    WebcamTool webcamTool;
    PhotoTool photoTool;
    PhotoAnimController photoAnimController;
    WebTool webTool;
    QRCodeGenerator qrCodeGenerator;

    [Header("Upload")]
    public string tempFolderName = "Booth";
    public bool ignoreUpload = false;

    [Header("UI")]
    public Button btn_takePhoto;
    public RawImage rImg_takenPhoto;

    [Header("UI - QR Code")]
    public GameObject qrCodePanel;
    public RawImage rImg_QRCodePhoto;
    public Texture loadingTexture;
    public Texture networkErrorTexture;
    public TextMeshProUGUI txt_scanQRCode;

    [Header("Hide During Capture")]
    public GameObject[] hideDuringCapture;

    void Start()
    {
        webcamTool = GetComponentInChildren<WebcamTool>();
        photoTool = GetComponentInChildren<PhotoTool>();
        photoAnimController = photoTool.gameObject.GetComponent<PhotoAnimController>();
        webTool = GetComponentInChildren<WebTool>();
        qrCodeGenerator = GetComponentInChildren<QRCodeGenerator>();

        if (webcamTool == null || photoTool == null || photoAnimController == null || webTool == null || qrCodeGenerator == null)
        {
            Debug.LogError("Missing Tools!");
        }

        webcamTool?.Init();
        // photoTool?.Init();
        photoAnimController?.Init();
    }

    void OnEnable()
    {
        //btn_takePhoto.onClick.AddListener(HandleBtnTakePhotoClicked);
        PhotoAnimController.OnCountDownFinish += HandleCountDownFinish;
        PhotoTool.OnPhotoTaken += HandlePhotoTaken;
        WebTool.OnUploadPhotoReqFinish += HandleUploadReqFinish;
        QRCodeGenerator.OnQRCodeGenerated += HandleQRCodeGenerated;
    }

    void OnDisable()
    {
        // btn_takePhoto.onClick.RemoveAllListeners();
        PhotoAnimController.OnCountDownFinish -= HandleCountDownFinish;
        PhotoTool.OnPhotoTaken -= HandlePhotoTaken;
        WebTool.OnUploadPhotoReqFinish -= HandleUploadReqFinish;
        QRCodeGenerator.OnQRCodeGenerated -= HandleQRCodeGenerated;
    }

    public void HandleBtnTakePhotoClicked()
    {
        // Hide the button
        btn_takePhoto.gameObject.SetActive(false);
        // Play count down animation
        photoAnimController.PlayCountDownAnim();
    }

    void HandleCountDownFinish()
    {
        Debug.Log("Count Down Finish!");
        foreach (GameObject go in hideDuringCapture)
        {
            go.SetActive(false);
        }
        photoTool.TakePhoto();
        webcamTool.PauseWebcam();
    }

    void HandleShotFinish()
    {
        Debug.Log("Shot Finish!");
    }

    void HandlePhotoTaken(Texture2D _takenPhoto)
    {
        photoAnimController.PlayShotAnim();

        // Use a subfolder in the user's temp directory
        string tempPath = Path.Combine(Path.GetTempPath(), tempFolderName);
        Directory.CreateDirectory(tempPath);
        Debug.Log("Photo save path: " + tempPath);

        // Save taken photo as a png file
        string filename = "photo_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        FileIOUtility.SaveImage(_takenPhoto, tempPath, filename, FileIOUtility.FileExtension.PNG);

        foreach (GameObject go in hideDuringCapture)
        {
            go.SetActive(true);
        }

        if (ignoreUpload)
        {
            StartCoroutine(E_WaitAndReset());
            return;
        }

        webTool.UploadPhoto(_takenPhoto, filename);
        qrCodePanel.SetActive(true);
        txt_scanQRCode.text = "Uploading Image ...";
        rImg_QRCodePhoto.texture = loadingTexture;
    }

    void HandleUploadReqFinish(bool _success, string _photoUrl)
    {
        if (_success)
        {
            if (_photoUrl != null)
            {
                // Generate QR Code
                txt_scanQRCode.text = "Generatic QR Code...";
                qrCodeGenerator.GenerateQRCode(_photoUrl);
            }
            else
                Debug.LogError("photoURL == null");
        }
        else
        {
            // Show Error Canvas
            Debug.LogError("Network ERROR!");
            txt_scanQRCode.text = "NETWORK ERROR! Try again!";
            rImg_QRCodePhoto.texture = networkErrorTexture;
        }

        // Wait for some time and go back to a playable status
        StartCoroutine(E_WaitAndReset());
    }

    void HandleQRCodeGenerated(Texture2D _generatedQRCode)
    {
        // Fake wait for user to feel that they are waiting on something :)
        StartCoroutine(E_WaitAndSetQRCode(_generatedQRCode));

        //// Save taken photo as a png file
        //photoFileName = FileIOUtility.GenerateFileName(photoFileNamePrefix, playerCount, FileIOUtility.FileExtension.PNG);
        //FileIOUtility.SaveImage(tex_takenPhoto, photoSavePath, photoFileName, FileIOUtility.FileExtension.PNG);

        //// Save QRCode
        //string qrFileName = FileIOUtility.GenerateFileName(qrFileNamePrefix, playerCount, FileIOUtility.FileExtension.PNG);
        //FileIOUtility.SaveImage(tex_qrCode, qrSavePath, qrFileName, FileIOUtility.FileExtension.PNG);

        //// Disable the loading icon
        //uiController.ActivateUploadingProcess(false);
    }

    IEnumerator E_WaitAndSetQRCode(Texture2D _generatedQRCode)
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3f));

        rImg_QRCodePhoto.texture = _generatedQRCode;
        txt_scanQRCode.text = "Scan Qr Code";
    }

    IEnumerator E_WaitAndReset()
    {
        yield return new WaitForSeconds(2f);

        // Show the button again
        btn_takePhoto.gameObject.SetActive(true);

        // Hide rImg_takenPhoto
        //rImg_takenPhoto.gameObject.SetActive(false);
    }
}
