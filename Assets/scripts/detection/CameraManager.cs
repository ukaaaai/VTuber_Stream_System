using System;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public class CameraManager
    {
        private readonly int _maxDevices;

        private const int Width = 768;
        private const int Height = 432;
        
        private WebCamTexture _webCamTexture;
        private string _currentDevice;

        //singleton
        private static CameraManager _instance;
        public static CameraManager Instance
        {
            get
            {
                return _instance ??= new CameraManager();
            }
        }
        
        private CameraManager()
        {
            var devices = WebCamTexture.devices;
            _currentDevice = devices[0].name;
            _webCamTexture = new WebCamTexture();
        }
        
        public static void Refresh ()
        {
            _instance = null;
            _instance = new CameraManager();
        }

        public void CameraOpen(string device)
        {
            if(_currentDevice == device)
                return;
            
            if(_webCamTexture != null) _webCamTexture.Stop();
            
            try
            {
                _webCamTexture = new WebCamTexture(device, Width, Height, 30);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
            _currentDevice = device;
            _webCamTexture.Play();
        }
        
        public void GetFrame(out Mat mat)
        {
            
            if (!_webCamTexture.isPlaying)
            {
                _webCamTexture = new WebCamTexture(_currentDevice, Width, Height, 30);
                _webCamTexture.Play();
            }
            var height = _webCamTexture.height;
            var width = _webCamTexture.width;

            mat = new Mat();
            Cv2.Resize(
                new Mat(height, width, MatType.CV_8UC4, _webCamTexture.GetPixels32()), 
                mat, 
                new Size(Width, Height));
            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2BGR);
        }

        private void Stop()
        {
            _webCamTexture.Stop();
        }

        public static void Release()
        {
            if(_instance == null)
                return;
            _instance.Stop();
            _instance = null;
        }

        public static void Init()
        {
            _instance ??= new CameraManager();
        }
    }
}