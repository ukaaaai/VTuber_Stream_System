using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DlibDotNet;
using Live2Dmodel;
using OpenCvSharp;
using UnityEngine;
using Point = DlibDotNet.Point;
using Rect = OpenCvSharp.Rect;

namespace detection
{
    public static class CoordinateParser
    {
        private static float _defaultEyeRatio = 0.3f;
        private const float DefaultBlowRatio = 1.5f;

        public static Param Parse(in Point[] landmarkPoints, in Array2D<RgbPixel> image, in Mat mat)
        {
            var points = PointToComp(landmarkPoints);
            var eyeRatio = GetEyeRatio(landmarkPoints).ToArray();
            var pupil = GetPupil(landmarkPoints, mat, eyeRatio);
            var blow = GetBlow(points).ToArray();
            var mouth = GetMouth(points).ToArray();
            
            var row = image.Rows;
            var col = image.Columns;
            var vec = SolvePnP.Solve(landmarkPoints, row, col);
            var rot = vec[1];/*new Vector3(
                (float)Math.Sin(vec[1].x), 
                (float)Math.Sin(vec[1].y), 
                (float)Math.Sin(vec[1].z));//*/

            return new Param()
            {
                ParamAngleX = rot.x,
                ParamAngleY = rot.y,
                ParamAngleZ = rot.z,
                ParamEyeLOpen = eyeRatio[0],
                ParamEyeROpen = eyeRatio[1],
                ParamEyeBallX = pupil.X,
                ParamEyeBallY = pupil.Y,
                ParamBrowLY = blow[0],
                ParamBrowRY = blow[1],
                ParamMouthForm = mouth[1],
                ParamMouthOpenY = mouth[0],
                ParamCheek = 0,
                ParamBreath = ModelManager.ParamBreath
            };
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

        private static Point GetPupil(in Point[] points, in Mat image, in float[] eyeRatio)
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
                var rect = MakeRect(points[43 .. 48]);
                var eye = image.SubMat(rect);
                Cv2.BitwiseNot(eye.Threshold(0, 255, ThresholdTypes.Otsu), eye);
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
                (float)Math.Clamp((points[63].Imaginary - points[31].Imaginary) / (points[67].Imaginary- points[63].Imaginary), -1, 1),
                (float)Math.Clamp((points[55].Real - points[49].Real) / (points[36].Real - points[32].Real) - 1, 0, 1)
            };
        }

        private static IEnumerable<float> GetAngles(in Complex[] points)
        {
            ref var nose = ref points[34];

            var left = (float)((points[1].Real + points[3].Real) / 2 - nose.Real);
            var right = (float)((points[15].Real + points[17].Real) / 2 - nose.Real);
            var angleX = Math.Clamp((left < right ? -(float)Math.Asin(left / right) : (float)Math.Asin(right / left))
                                    * Mathf.Rad2Deg, -30, 30);

            return new float[]{};
        }
    }
}
