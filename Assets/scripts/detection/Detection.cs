using System;
using OpenCvSharp;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace detection
{
    public class Detection : MonoBehaviour
    {
        [Serializable] private class CameraFrame:UnityEvent<Mat> { }
        [SerializeField] private CameraFrame onCameraFrame = new();
        
        private bool _isRunning;
        
        private static FrontalFaceDetector _faceDetector;
        private static ShapePredictor _shapePredictor;
        
        private void Start()
        {
            _faceDetector = Dlib.GetFrontalFaceDetector();
            _shapePredictor = ShapePredictor.Deserialize("Assets/scripts/shape_predictor_68_face_landmarks.dat");
        }
        private void Update()
        {
            if (!_isRunning) return;
            GetLandmarks();
        }
        
        public void StartDetection()
        {
            _isRunning = true;
        }
        
        public void StopDetection()
        {
            _isRunning = false;
        }

        public void GetLandmarks()
        {
            if(!_isRunning) return;
            CameraManager.Instance.GetFrame(out var mat);
            Cv2.Flip(mat, mat, FlipMode.X);
            var width = mat.Width;
            var height = mat.Height;
            var array = new byte[width * height * mat.ElemSize()];
            Marshal.Copy(mat.Data, array, 0, array.Length);
            var image = Dlib.LoadImageData<RgbPixel>(
                array,
                (uint)mat.Height, 
                (uint)mat.Width,
                (uint)(mat.Width * mat.ElemSize()));
                
            var faces = _faceDetector.Operator(image);
                
            if(faces.Length == 0) return;
                
            var points = new DlibDotNet.Point[68];
                
            var shapes = _shapePredictor.Detect(image, faces[0]);
            Parallel.For(0, 68, i =>
            {
                points[i] = shapes.GetPart((uint)i);
                Cv2.Circle(mat,
                    new OpenCvSharp.Point(points[i].X, points[i].Y),
                    2,
                    Scalar.Green,
                    -1);
            });
            Cv2.Flip(mat, mat, FlipMode.X);
            onCameraFrame.Invoke(mat);
        }
    }
}
