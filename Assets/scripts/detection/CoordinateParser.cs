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
        private static float _defaultEyeRatio = 0.3f;
        private const float DefaultBlowRatio = 1.5f;

        public static Param Parse(in Complex[] landmarkPoints, in Array2D<RgbPixel> image, in Mat mat)
        {
            var eyeRatio = GetEyeRatio(landmarkPoints).ToArray();
            var pupil = GetPupil(landmarkPoints, mat, eyeRatio);
            var blow = GetBlow(landmarkPoints).ToArray();
            var mouth = GetMouth(landmarkPoints).ToArray();
            var rot = SolvePnP.Solve(landmarkPoints, image.Rows, image.Columns);

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
            var left1 = points[38] - points[36];
            var left2 = points[40] - points[36];
            var left3 = points[37] - points[39];
            var left4 = points[41] - points[39];
            var leftRatio = (float)Math.Sin(Math.Abs(left1.Phase - left2.Phase) + Math.Abs(left3.Phase - left4.Phase) / 2);
            
            var right1 = points[44] - points[42];
            var right2 = points[46] - points[42];
            var right3 = points[43] - points[45];
            var right4 = points[47] - points[45];
            var rightRatio = (float)Math.Sin(Math.Abs(right1.Phase - right2.Phase) + Math.Abs(right3.Phase - right4.Phase) / 2);

            _defaultEyeRatio = Math.Min(Math.Max(_defaultEyeRatio, Math.Max(leftRatio * 0.8f, rightRatio * 0.8f)), 1.0f);
            
            return new[] { Math.Min(leftRatio / _defaultEyeRatio, 1.0f), Math.Min(rightRatio / _defaultEyeRatio, 1.0f)};
        }

        private static Point GetPupil(in Complex[] points, in Mat image, in float[] eyeRatio)
        {
            var pupils = new []
            {
                new Point(-1, -1),
                new Point(-1, -1)
            };
            
            if (eyeRatio[0] > 0.3f)
            {
                var rect = MakeRect(points[36 .. 41]);
                var eye = image.SubMat(rect);
                Cv2.CvtColor(eye, eye, ColorConversionCodes.BGR2GRAY);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                pupils[0] = new Point(2 * x / rect.X - 1, 2 * y / rect.Y - 1);
            }

            if (!(eyeRatio[1] > 0.3f))
                return new Point(Math.Max(pupils[0].X, pupils[1].X), Math.Max(pupils[0].Y, pupils[1].Y));
            {
                var rect = MakeRect(points[42 .. 47]);
                var eye = image.SubMat(rect);
                eye = eye.Threshold(0, 255, ThresholdTypes.Otsu);
                Cv2.BitwiseNot(eye, eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                pupils[1] = new Point(2 * x / rect.X - 1, 2 * y / rect.Y - 1);
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
