using System.IO;
using System.Numerics;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public sealed class Detection
    {
        private static FrontalFaceDetector _faceDetector;
        private static ShapePredictor _shapePredictor;

        private static Detection _instance;
        public static Detection Instance
        {
            get
            {
                return _instance ??= new Detection();
            }
        }

        public static void Init()
        {
            _instance = null;
            _instance = new Detection();
        }

        private Detection()
        {
            _faceDetector = Dlib.GetFrontalFaceDetector();
            using var fs = File.OpenRead(Application.dataPath + "/StreamingAssets/shape_predictor_68_face_landmarks.dat");
            var bytes = new byte[fs.Length];
            // ReSharper disable once MustUseReturnValue
            fs.Read(bytes, 0, bytes.Length);
            _shapePredictor = ShapePredictor.Deserialize(bytes);
        }

        public Param? Detect(Mat mat)
        {
            var elemSize = mat.ElemSize();
            
            var array = new byte[CameraManager.Width * CameraManager.Height * elemSize];
            Marshal.Copy(mat.Data, array, 0, array.Length);
            
            var image = Dlib.LoadImageData<RgbPixel>(
                array,
                (uint)CameraManager.Height, 
                (uint)CameraManager.Width,
                (uint)(CameraManager.Width * elemSize));
            
            var faces = _faceDetector.Operator(image);

            if (faces.Length is 0) return null;

            var shapes = _shapePredictor.Detect(image, faces[0]);
            
            var points = new Complex[68];
            Parallel.For(0, 68, i =>
            {
                var point = shapes.GetPart((uint)i);
                points[i] = new Complex(point.X, point.Y);
            });
            return CoordinateParser.Parse(points, image, mat);
        }
    }
}
