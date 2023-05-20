using System;
using System.IO;
using System.Numerics;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace detection
{
    public sealed class Detection : MonoBehaviour
    {
        [Serializable] private class DetectParam: UnityEvent<Param> { }
        [SerializeField] private DetectParam onDetect = new();

        private bool _isRunning;
        
        private static FrontalFaceDetector _faceDetector;
        private static ShapePredictor _shapePredictor;
        private int _width;
        private int _height;
        
        private void Start()
        {
            _faceDetector = Dlib.GetFrontalFaceDetector();
            using var fs = File.OpenRead(Application.dataPath + "/StreamingAssets/shape_predictor_68_face_landmarks.dat");
            var bytes = new byte[fs.Length];
            // ReSharper disable once MustUseReturnValue
            fs.Read(bytes, 0, bytes.Length);
            _shapePredictor = ShapePredictor.Deserialize(bytes);
            _width = CameraManager.Width;
            _height = CameraManager.Height;
        }
        private void Update()
        {
            Detect();
        }
        
        public void StartDetection() => _isRunning = true;
        
        public void StopDetection() => _isRunning = false;

        private void Detect()
        {
            if(!_isRunning) return;
            CameraManager.Instance.GetFrame(out var mat);
            
            var elemSize = mat.ElemSize();
            
            var array = new byte[_width * _height * elemSize];
            Marshal.Copy(mat.Data, array, 0, array.Length);
            mat.Dispose();
            
            var image = Dlib.LoadImageData<RgbPixel>(
                array,
                (uint)_height, 
                (uint)_width,
                (uint)(_width * elemSize));
            
            var faces = _faceDetector.Operator(image);

            if (faces.Length is 0) return;

            var shapes = _shapePredictor.Detect(image, faces[0]);
            
            var points = new Complex[68];
            Parallel.For(0, 68, i =>
            {
                var point = shapes.GetPart((uint)i);
                points[i] = new Complex(point.X, point.Y);
            });
            var param = CoordinateParser.Parse(points, image, mat);
            onDetect.Invoke(param);
        }
    }
}
