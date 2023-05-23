using System;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public sealed class CameraManager
    {
        private readonly int _maxDevices;

        public static int Width => 640;
        public static int Height => 360;

        private WebCamTexture _webCamTexture;
        private string _currentDevice; 
        
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
            if(_currentDevice == device) return;
            
            if(_webCamTexture != null) _webCamTexture.Stop();
            _webCamTexture = new WebCamTexture(device, Width, Height, 30);
            
            _currentDevice = device;
        }
        
        public Mat GetFrame()
        {
            if (!_webCamTexture.isPlaying)
            {
                _webCamTexture = new WebCamTexture(_currentDevice, Width, Height, 30);
                _webCamTexture.Play();
            }
            var height = _webCamTexture.height;
            var width = _webCamTexture.width;

            var mat = new Mat(height, width, MatType.CV_8UC4, _webCamTexture.GetPixels32());
            Cv2.Resize(
                mat, 
                mat, 
                new Size(Width, Height));
            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2BGR);
            Cv2.Flip(mat, mat, FlipMode.X);
            return mat;
        }

        public void Start()
        {
            if(_webCamTexture != null)
                _webCamTexture.Play();
        }

        public void Stop()
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