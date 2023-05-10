using System;
using System.Numerics;
using OpenCvSharp;

namespace detection
{
    public static class HeadPoseEstimation
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
            { 0.0f, 250.0f, -50.0f }
        };

        public static Vector3 Solve(in Complex[] points, in int row, in int col)
        {
            var imagePoints = new [,]
            {
                { points[30].Real, points[30].Imaginary },
                { points[21].Real, points[21].Imaginary },
                { points[22].Real, points[22].Imaginary },
                { points[39].Real, points[39].Imaginary },
                { points[42].Real, points[42].Imaginary },
                { points[31].Real, points[31].Imaginary },
                { points[35].Real, points[35].Imaginary },
                { points[48].Real, points[48].Imaginary },
                { points[54].Real, points[54].Imaginary },
                { points[57].Real, points[57].Imaginary },
                { points[8].Real, points[8].Imaginary },
            };
            var cameraMatrix = new[,]{{col, 0, col / 2.0}, {0, col, row / 2.0}, {0, 0, 1}};
            var cameraMatrixMat = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix);
            
            var rVec = new Mat();
            var tVec = new Mat();
            Cv2.SolvePnP(InputArray.Create(ModelPoints), 
                InputArray.Create(imagePoints), 
                cameraMatrixMat, 
                new Mat(4, 1, MatType.CV_64FC1, 0), 
                rVec, 
                tVec);

            var rMat = new Mat();
            Cv2.Rodrigues(rVec, rMat);
            
            var projMat = new Mat();
            Cv2.HConcat(rMat, new Mat(3, 1, MatType.CV_64FC1, 0), projMat);
            var rotMat = new Mat();
            var eulerAngle = new Mat();

            Cv2.DecomposeProjectionMatrix(projMat, 
                cameraMatrixMat, 
                rMat,
                tVec,
                rotMat,
                rotMat,
                rotMat,
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
