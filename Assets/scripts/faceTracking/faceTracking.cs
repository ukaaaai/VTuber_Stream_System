using UnityEngine;
using System;
using DlibDotNet;
using OpenCvSharp;

public class faceTracking : MonoBehaviour
{
    [SerializeField] static int width = 1920;
    [SerializeField] static int height = 1080;
    [SerializeField] static int fps = 60;
    private static WebCamTexture webCamTexture;
    private static WebCamDevice webCamDevice;
    private faceTracking() { }

    public static void trackingStart()
    {

    }

    public static void changeDevice(WebCamDevice device)
    {
        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
        WebCamDevice deviceTmp = webCamDevice;
        webCamDevice = device;
        try
        {
            webCamTexture = new WebCamTexture(webCamDevice.name, width, height, fps);
        }
        catch (Exception)
        {
            webCamDevice = deviceTmp;
            webCamTexture = new WebCamTexture(webCamTexture.name, width, height, fps);

            throw new Exception("change faild");
        }
    }
}
