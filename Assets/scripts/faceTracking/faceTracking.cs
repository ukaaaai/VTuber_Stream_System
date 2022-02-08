using UnityEngine;
using System;

public class faceTracking : MonoBehaviour
{
    [SerializeField] int width = 1920;
    [SerializeField] int height = 1080;
    [SerializeField] int fps = 60;
    private static WebCamTexture webCamTexture;
    private static WebCamDevice webCamDevice;

    private static faceTracking _Instance = new faceTracking();
    private faceTracking() { }

    public static faceTracking getInstance()
    {
        return _Instance;
    }

    public void trackingStart()
    {

    }

    public void changeDevice(WebCamDevice device)
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
