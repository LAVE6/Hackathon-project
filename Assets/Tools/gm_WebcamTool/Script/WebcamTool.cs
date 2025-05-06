using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebcamTool : MonoBehaviour
{

    //public string deviceName = "USB Camera";

    public int width = 1920;
    public int height = 1080;

    public Material targetMaterial;

    public bool IsCamReady { get { return isCamReady; } }
    bool isCamReady;

    WebCamTexture webcamTexture;
    WebCamDevice[] devices;

    public void Init()
    {
        devices = WebCamTexture.devices;

        // List all webcam devices
        // ListAllDevices();

        if (devices.Length == 0)
        {
            Debug.LogError("Please supply a valid device name");
            return;
        }

        string deviceName = devices[0].name;
        webcamTexture = new WebCamTexture(deviceName);
        webcamTexture.Play();

        // Check if cam is opened
        if (webcamTexture && webcamTexture.isPlaying)
        {
            isCamReady = true;
            Debug.Log("Opened " + deviceName + " with " + webcamTexture.width + ", " + webcamTexture.height);
        }
        else
        {
            isCamReady = false;
            Debug.LogError("Webcam Not Opened: " + deviceName);
        }

        // If targetMaterial is not null, automatically replace its texture with webcamTexture
        if (targetMaterial != null)
            SetToMaterial(targetMaterial);

        //PauseWebcam();
    }

    void ListAllDevices()
    {
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.LogWarning("Device detected: " + devices[i].name);
        }
    }

    public Texture GetWebcamTex()
    {
        if (isCamReady)
            return webcamTexture;
        else
        {
            Debug.LogError("WebcamTexture is not ready");
            return null;
        }
    }

    public void SetToMaterial(Material _material)
    {
        _material.SetTexture("_MainTex", GetWebcamTex());
    }

    public void PauseWebcam()
    {
        if (webcamTexture.isPlaying)
            webcamTexture.Pause();
    }

    public void PlayWebcam()
    {
        if (webcamTexture.isPlaying == false)
            webcamTexture.Play();
    }
}
