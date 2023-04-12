using System;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public struct SolvePnP
    {
        private static readonly float[,] ModelPoints = {
            { 0.0f, 0.0f, 0.0f },
            { -30.0f, -125.0f, -30.0f },
            { 30.0f, -125.0f, -30.0f },
            { -60.0f, -70.0f, -60.0f },
            { 60.0f, -70.0f, -60.0f },
            { -40.0f, 40.0f, -50.0f },
            { 40.0f, 40.0f, -50.0f },
            { -70.0f, 130.0f, -100.0f },
            { 70.0f, 130.0f, -100.0f },
            { 0.0f, 158.0f, -10.0f },
            {0.0f, 250.0f, -50.0f}
        };

        public static Vector3 Solve(in DlibDotNet.Point[] points, in int row, in int col)
        {
            var imagePoints = new float[,]
            {
                { points[30].X, points[30].Y },
                { points[21].X, points[21].Y },
                { points[22].X, points[22].Y },
                { points[39].X, points[39].Y },
                { points[42].X, points[42].Y },
                { points[31].X, points[31].Y },
                { points[35].X, points[35].Y },
                { points[48].X, points[48].Y },
                { points[54].X, points[54].Y },
                { points[57].X, points[57].Y },
                { points[8].X, points[8].Y },
            };
            var distCoeffs = new Mat(4, 1, MatType.CV_64FC1, 0);
            var center = new Point2d(col / 2.0, row / 2.0);
            var cameraMatrix = new[,]{{col, 0, center.X}, {0, col, center.Y}, {0, 0, 1}};
            var cameraMatrixMat = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix);
            
            var rVec = new Mat();
            var tVec = new Mat();
            Cv2.SolvePnP(InputArray.Create(ModelPoints), 
                InputArray.Create(imagePoints), 
                cameraMatrixMat, 
                distCoeffs, 
                rVec, 
                tVec);

            var rMat = new Mat();
            Cv2.Rodrigues(rVec, rMat);
            
            var projMat = new Mat();
            Cv2.HConcat(rMat, new Mat(3, 1, MatType.CV_64FC1, 0), projMat);
            var rotMatX = new Mat();
            var rotMatY = new Mat();
            var rotMatZ = new Mat();
            var eulerAngle = new Mat();

            Cv2.DecomposeProjectionMatrix(projMat, 
                cameraMatrixMat, 
                rVec, 
                tVec,
                rotMatX,
                rotMatY,
                rotMatZ,
                eulerAngle);
            
            var pitch = (float)eulerAngle.At<double>(0, 0);
            var yaw = (float)eulerAngle.At<double>(1, 0);
            var roll = (float)eulerAngle.At<double>(2, 0);

            pitch = Math.Clamp(Math.Sign(pitch) * Math.Min(Math.Abs(pitch), 180 - Math.Abs(pitch)) * 2, -30, 30);
            yaw = Math.Clamp(Math.Sign(yaw) * Math.Min(Math.Abs(yaw) * 2, 180 -Math.Abs(yaw)), -30, 30);
            roll = Math.Clamp(Math.Sign(roll) * Math.Min(Math.Abs(roll) * 2, 180 - Math.Abs(roll)), -30, 30);

            var rotVecEuler = new Vector3(yaw, pitch, roll);

            return rotVecEuler;
        }
    }
}
