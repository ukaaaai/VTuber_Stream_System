using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using Point = DlibDotNet.Point;

namespace detection
{
    public static class CoordinateParser
    {
        private static float _defaultEyeRatio = 0.3f;
        private static Rect MakeRect(IEnumerable<Point> points)
        {
            var enumerable = points as Point[] ?? points.ToArray();
            var left = enumerable.Min(t => t.X);
            var top = enumerable.Min(t => t.Y);
            var width = enumerable.Max(t => t.X) - left;
            var height = enumerable.Max(t => t.Y) - top;

            return new Rect(left, top, width, height);
        }

        public static IEnumerable<float> GetEyeRatio(in Point[] points)
        {
            var left1 = points[39] - points[37];
            var left2 = points[41] - points[37];
            var left3 = points[38] - points[40];
            var left4 = points[42] - points[40];
            var leftRatio = Math.Sin(
                (Math.Abs(Math.Acos(left1.X * left2.X + left1.Y * left2.Y) / (left1.Length + left2.Length)) +
                 Math.Abs(Math.Acos(left3.X * left4.X + left3.Y * left4.Y) / (left3.Length + left4.Length))) / 2);
            
            var right1 = points[45] - points[43];
            var right2 = points[47] - points[43];
            var right3 = points[44] - points[46];
            var right4 = points[48] - points[46];
            var rightRatio = Math.Sin(
                (Math.Abs(Math.Acos(right1.X * right2.X + right1.Y * right2.Y) / (right1.Length + right2.Length)) +
                 Math.Abs(Math.Acos(right3.X * right4.X + right3.Y * right4.Y) / (right3.Length + right4.Length))) / 2); 

            _defaultEyeRatio = (float)Math.Max(_defaultEyeRatio, Math.Max(leftRatio * 0.8, rightRatio * 0.8));
            
            return new[] { Math.Min((float)leftRatio / _defaultEyeRatio, 1.0f),
                Math.Min((float)rightRatio / _defaultEyeRatio, 1.0f)};
        }

        public static IEnumerable<Point> GetPupil(in Point[] points, in Mat image, in float[] eyeRatio)
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
                
                pupils[0] = new Point(rect.X + x, rect.Y + y);
            }

            if (!(eyeRatio[1] > 0.3f)) return pupils;
            {
                var rect = MakeRect(points[43 .. 48]);
                var eye = image.SubMat(rect);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
                var moments = eye.Moments(true);
                var x = (int)(moments.M10 / moments.M00);
                var y = (int)(moments.M01 / moments.M00);
                
                pupils[1] = new Point(rect.X + x, rect.Y + y);
            }

            return pupils;
        }
    }
}
