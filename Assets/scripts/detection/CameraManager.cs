using OpenCvSharp;
using System.Threading.Tasks;
using UnityEngine;

namespace detection
{
    public class CameraManager
    {
        private readonly int _maxDevices;

        private const int Width = 960;
        private const int Height = 540;
        
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
            _currentDevice = device;
            _webCamTexture = new WebCamTexture(device, Width, Height, 30);
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
            var c = _webCamTexture.GetPixels32();
            var vec3B = new Vec3b[c.Length];

            Parallel.For(0, height, i =>
            {
                for(var j = 0; j < width; j++)
                {
                    var n = i * width + j;
                    vec3B[n] = new Vec3b(c[n].b, c[n].g, c[n].r);
                }
            });
            mat = new Mat(height, width, MatType.CV_8UC3, vec3B);
            Cv2.Resize(mat, mat, new Size(Width, Height));
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