using System;
using OpenCvSharp;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace detection
{
    public sealed class Detection : MonoBehaviour
    {
        [Serializable] private class DetectPreview: UnityEvent<Mat> { }
        [SerializeField] private DetectPreview onPreview = new();
        [Serializable] private class DetectParam: UnityEvent<Vec3f, DlibDotNet.Point[]> { }
        [SerializeField] private DetectParam onDetect = new();
        
        private bool _isRunning;
        
        private static FrontalFaceDetector _faceDetector;
        private static ShapePredictor _shapePredictor;

        private static int _cantFindFrames;
        private static Rectangle _face = Rectangle.Empty;
        
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

            if (faces.Length == 0)
            {
                if (_cantFindFrames >= 10)
                {
                    return;
                }
                if (_face == Rectangle.Empty)
                {
                    return;
                }

                _cantFindFrames++;
            }
            else
            {
                _face = faces[0];
                _cantFindFrames = 0;
            }
            
            var points = new DlibDotNet.Point[68];

            var shapes = _shapePredictor.Detect(image, _face);
            Parallel.For(0, 68, i =>
            {
                points[i] = shapes.GetPart((uint)i);
                Cv2.Circle(mat,
                    new OpenCvSharp.Point(points[i].X, points[i].Y),
                    2,
                    Scalar.Green,
                    -1);//*/
            });//*/
            var row = image.Rows;
            var col = image.Columns;
            var vec = SolvePnP.Solve(points, row, col);
            var rot = new Vec3f(
                (float)Math.Sin(vec[1].x), 
                (float)Math.Sin(vec[1].y), 
                (float)Math.Sin(vec[1].z));
            Cv2.Flip(mat, mat, FlipMode.X);
            onDetect.Invoke(rot, points);
            onPreview.Invoke(mat); 
        }
    }
}
