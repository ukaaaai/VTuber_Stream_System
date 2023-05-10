using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DlibDotNet;
using OpenCvSharp;
using Point = OpenCvSharp.Point;

namespace detection
{
    public static class CoordinateParser
    {
        private const float DefaultBlowRatio = 1.5f;

        public static Param Parse(in Complex[] landmarkPoints, in Array2D<RgbPixel> image, in Mat mat)
        {
            var eyeRatio = GetEyeRatio(landmarkPoints).ToArray();
            var pupil = GetPupil(landmarkPoints, mat, eyeRatio);
            var blow = GetBlow(landmarkPoints).ToArray();
            var mouth = GetMouth(landmarkPoints).ToArray();
            var rot = HeadPoseEstimation.Solve(landmarkPoints, image.Rows, image.Columns);

            return new Param()
            {
                ParamAngleX = rot.X,
                ParamAngleY = rot.Y,
                ParamAngleZ = rot.Z,
                ParamEyeLOpen = eyeRatio[0],
                ParamEyeROpen = eyeRatio[1],
                ParamEyeBallX = pupil.X,
                ParamEyeBallY = pupil.Y,
                ParamBrowLY = blow[0],
                ParamBrowRY = blow[1],
                ParamMouthForm = mouth[1],
                ParamMouthOpenY = mouth[0],
                ParamCheek = 0,
                ParamBreath = Live2Dmodel.ModelManager.ParamBreath
            };
        }

        private static Rect MakeRect(IEnumerable<Complex> points)
        {
            var enumerable = points as Complex[] ?? points.ToArray();
            var left = (int)enumerable.Min(t => t.Real);
            var top = (int)enumerable.Min(t => t.Imaginary);
            var width = (int)enumerable.Max(t => t.Real) - left;
            var height = (int)enumerable.Max(t => t.Imaginary) - top;

            return new Rect(left, top, width, height);
        }

        private static IEnumerable<float> GetEyeRatio(in Complex[] points)
        {
            var leftRatio =
                (float)Math.Abs((points[37].Imaginary - points[41].Imaginary) / (points[38].Real - points[37].Real));
            
            var rightRatio = 
                (float)Math.Abs((points[44].Imaginary - points[46].Imaginary) / (points[43].Real - points[44].Real));
            
            return new[]
            {
                Math.Clamp(leftRatio / 0.8f, 0, 1),
                Math.Clamp(rightRatio / 0.8f, 0, 1)
            };
        }

        private static Point GetPupil(in Complex[] points, in Mat image, in float[] eyeRatio)
        {
            var pupils = new []
            {
                new Point(-1, -1),
                new Point(-1, -1)
            };

            var calPupil = new Func<Complex[], Mat, int, int, Point>((points, image, a, b) =>
            {
                var rect = MakeRect(points[a .. b]);
                var eye = image.SubMat(rect);
                Cv2.CvtColor(eye, eye, ColorConversionCodes.BGR2GRAY);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                return new Point(2 * x / rect.X - 1, 2 * y / rect.Y - 1);
            });
            
            if (eyeRatio[0] > 0.3f)
            {
                pupils[0] = calPupil(points, image, 36, 41);
            }

            // ReSharper disable once InvertIf
            if (eyeRatio[1] > 0.3f)
            {
                pupils[1] = calPupil(points, image, 42, 47);
            }

            return new Point(Math.Max(pupils[0].X, pupils[1].X), Math.Max(pupils[0].Y, pupils[1].Y));
        }

        private static IEnumerable<float> GetBlow(in Complex[] points)
        {
            return new[]
            {
                (float)Math.Clamp(
                    DefaultBlowRatio - (points[17] - points[30]).Magnitude / (points[36] - points[30]).Magnitude,
                    -1, 1),
                (float)Math.Clamp(
                    DefaultBlowRatio - (points[26] - points[30]).Magnitude / (points[45] - points[30]).Magnitude,
                    -1, 1)
            };
        }

        private static IEnumerable<float> GetMouth(in Complex[] points)
        {
            return new[]
            {
                (float)Math.Clamp((points[62].Imaginary - points[30].Imaginary) / (points[66].Imaginary- points[62].Imaginary), -1, 1),
                (float)Math.Clamp((points[54].Real - points[48].Real) / (points[35].Real - points[31].Real) - 1, 0, 1)
            };
        }
    }
}
