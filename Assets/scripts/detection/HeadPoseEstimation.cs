using System;
using System.Numerics;
using OpenCvSharp;
using Vector3 = System.Numerics.Vector3;

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

        private static readonly Mat RMat = new();
        private static readonly Mat RVec = new();
        private static readonly Mat Vec = new();

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
            
            var cameraMatrix = new[,]
            {
                {col, 0, col / 2.0}, 
                {0, col, row / 2.0}, 
                {0, 0, 1}
            };
            var cameraMatrixMat = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix);
            
            Cv2.SolvePnP(InputArray.Create(ModelPoints), 
                InputArray.Create(imagePoints), 
                cameraMatrixMat, 
                new Mat(4, 1, MatType.CV_64FC1, 0), 
                RVec, 
                Vec);

            Cv2.Rodrigues(RVec, RMat);
            
            const float r2d = 180 / (float)Math.PI;

            var yaw = Math.Clamp((float)Math.Asin(-RMat.At<double>(2, 0)) * r2d, -30, 30);
            var pitch = Math.Clamp((float)Math.Atan(RMat.At<double>(2, 1) / RMat.At<double>(2, 2)) * r2d, -30, 30);
            var roll = Math.Clamp((float)Math.Atan(RMat.At<double>(1, 0) / RMat.At<double>(0, 0)) * r2d, -30, 30);

            var rotVecEuler = new Vector3(yaw, pitch, roll);

            return rotVecEuler;
        }
    }
}
