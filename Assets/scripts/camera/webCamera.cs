using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class webCamera : MonoBehaviour
{
    [SerializeField] private RawImage rawImage = null;
    [SerializeField] private int width = 1080;
    [SerializeField] private int height = 1920;
    [SerializeField] private int fps = 60;

    private WebCamTexture webCamTexture;

    void Awake()
    {
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, width, height, fps);
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    public void changeDevice(WebCamDevice device)
    {
        try
        {
            webCamTexture.Stop();
        }
        catch(Exception) { }
        webCamTexture = new WebCamTexture(device.name, width, height, fps);
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }
}
