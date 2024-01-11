using System.IO;
using Util;
using DlibDotNet;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using OpenCvSharp;

namespace detection
{
    public class DlibDetector : IDetector
    {
        private static readonly FrontalFaceDetector FaceDetector;
        private static readonly ShapePredictor ShapePredictor;

        static DlibDetector()
        {
            FaceDetector = Dlib.GetFrontalFaceDetector();
            using var fs = File.OpenRead(UnityEngine.Application.dataPath + "/StreamingAssets/shape_predictor_68_face_landmarks.dat");
            var bytes = new byte[fs.Length];
            // ReSharper disable once MustUseReturnValue
            fs.Read(bytes, 0, bytes.Length);
            ShapePredictor = ShapePredictor.Deserialize(bytes);
        }

        public void Init(){}

        public Param? DetectTask(in Mat mat)
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
            for(uint i = 0; i < 68; i++)
            {
                var point = shapes.GetPart(i);
                points[i] = new Complex(point.X, point.Y);
            }
            return CoordinateParser.Parse(points, (CameraManager.Height, CameraManager.Width), mat);
        }

        public UniTask<Param?> Detect(in Mat mat)
        {
            return new UniTask<Param?>(DetectTask(mat));
        }
    }
}
