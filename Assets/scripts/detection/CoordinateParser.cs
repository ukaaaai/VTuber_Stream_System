using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenCvSharp;
using Point = DlibDotNet.Point;

namespace detection
{
    public static class CoordinateParser
    {
        private static float _defaultEyeRatio = 0.3f;
        private const float DefaultBlowRatio = 1.5f;

        public static void Parse(in Point[] landmarkPoints, in Mat image)
        {
            var points = PointToComp(landmarkPoints);
            var eyeRatio = GetEyeRatio(landmarkPoints).ToArray();
            var pupil = GetPupil(landmarkPoints, image, eyeRatio).ToArray();
            var blow = GetBlow(points).ToArray();
            var mouth = GetMouth(points).ToArray();
        }
        
        private static Complex[] PointToComp(in Point[] points)
        {
            return points.Select(t => new Complex(t.X, t.Y)).ToArray();
        }

        private static Rect MakeRect(IEnumerable<Point> points)
        {
            var enumerable = points as Point[] ?? points.ToArray();
            var left = enumerable.Min(t => t.X);
            var top = enumerable.Min(t => t.Y);
            var width = enumerable.Max(t => t.X) - left;
            var height = enumerable.Max(t => t.Y) - top;

            return new Rect(left, top, width, height);
        }

        private static IEnumerable<float> GetEyeRatio(in Point[] points)
        {
            var compPoints = PointToComp(points);
            
            var left1 = compPoints[39] - compPoints[37];
            var left2 = compPoints[41] - compPoints[37];
            var left3 = compPoints[38] - compPoints[40];
            var left4 = compPoints[42] - compPoints[40];
            var leftRatio = (float)Math.Sin(Math.Abs(left1.Phase - left2.Phase) + Math.Abs(left3.Phase - left4.Phase) / 2);
            
            var right1 = compPoints[45] - compPoints[43];
            var right2 = compPoints[47] - compPoints[43];
            var right3 = compPoints[44] - compPoints[46];
            var right4 = compPoints[48] - compPoints[46];
            var rightRatio = (float)Math.Sin(Math.Abs(right1.Phase - right2.Phase) + Math.Abs(right3.Phase - right4.Phase) / 2);

            _defaultEyeRatio = Math.Min(Math.Max(_defaultEyeRatio, Math.Max(leftRatio * 0.8f, rightRatio * 0.8f)), 1.0f);
            
            return new[] { Math.Min(leftRatio / _defaultEyeRatio, 1.0f), Math.Min(rightRatio / _defaultEyeRatio, 1.0f)};
        }

        private static IEnumerable<Point> GetPupil(in Point[] points, in Mat image, in float[] eyeRatio)
        {
            var pupils = new []
            {
                new Point(-1, -1),
                new Point(-1, -1)
            };
            
            if (eyeRatio[0] > 0.3f)
            {
                var rect = MakeRect(points[37 .. 42]);
                var eye = image.SubMat(rect);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                pupils[0] = new Point(2 * x / rect.X - 1, 2 * y / rect.Y - 1);
            }

            if (!(eyeRatio[1] > 0.3f)) return pupils;
            {
                var rect = MakeRect(points[43 .. 48]);
                var eye = image.SubMat(rect);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                pupils[1] = new Point(2 * x / rect.X - 1, 2 * y / rect.Y - 1);
            }

            return pupils;
        }

        private static IEnumerable<float> GetBlow(in Complex[] points)
        {
            return new[]
            {
                (float)Math.Clamp(
                    DefaultBlowRatio - (points[18] - points[31]).Magnitude / (points[37] - points[31]).Magnitude,
                    -1, 1),
                (float)Math.Clamp(
                    DefaultBlowRatio - (points[27] - points[31]).Magnitude / (points[46] - points[31]).Magnitude,
                    -1, 1)
            };
        }

        private static IEnumerable<float> GetMouth(in Complex[] points)
        {
            return new[]
            {
                (float)Math.Clamp((points[63] - points[31]).Imaginary / (points[67] - points[63]).Imaginary, -1, 1),
                (float)Math.Clamp((points[55] - points[49]).Real / (points[36] - points[32]).Real - 1, 0, 1)
            };
        }
    }
}
