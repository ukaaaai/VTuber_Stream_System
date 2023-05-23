using System.IO;
using Util;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public static class Detection
    {
        private static readonly FrontalFaceDetector FaceDetector;
        private static readonly ShapePredictor ShapePredictor;

        static Detection()
        {
            FaceDetector = Dlib.GetFrontalFaceDetector();
            using var fs = File.OpenRead(Application.dataPath + "/StreamingAssets/shape_predictor_68_face_landmarks.dat");
            var bytes = new byte[fs.Length];
            // ReSharper disable once MustUseReturnValue
            fs.Read(bytes, 0, bytes.Length);
            ShapePredictor = ShapePredictor.Deserialize(bytes);
        }

        public static void Init(){}

        public static Param? Detect(in Mat mat)
        {
            var steps = mat.ElemSize() * CameraManager.Width;
            
            var array = new byte[CameraManager.Height * steps];
            Marshal.Copy(mat.Data, array, 0, array.Length);
            
            using var image = Dlib.LoadImageData<RgbPixel>(
                array,
                (uint)CameraManager.Height, 
                (uint)CameraManager.Width,
                (uint)steps);
            
            var faces = FaceDetector.Operator(image);
            if (faces.Length is 0) return null;

            var shapes = ShapePredictor.Detect(image, faces[0]);
            var points = new Complex[68];
            Parallel.For(0, 68, i =>
            {
                var point = shapes.GetPart((uint)i);
                points[i] = new Complex(point.X, point.Y);
            });
            return CoordinateParser.Parse(points, (CameraManager.Height, CameraManager.Width), mat);
        }
    }
}
