using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour
{
    [Serializable] class CameraChangeEvent : UnityEvent<WebCamDevice> { }

    [SerializeField] private CameraChangeEvent cameraChangeEvent;
    [SerializeField] private UnityEngine.UI.Dropdown dropdown = null;

    private WebCamDevice[] devices;

    void Awake()
    {
        dropdown.ClearOptions();
        devices = WebCamTexture.devices;
        List<string> cameraNames = new List<string>();
        foreach(var device in devices)
        {
            cameraNames.Add(device.name);
        }
        dropdown.AddOptions(cameraNames);

        sendDevice();
    }

    public void sendDevice()
    {
        WebCamDevice device = devices[dropdown.value];
        cameraChangeEvent.Invoke(device);
    }
}
