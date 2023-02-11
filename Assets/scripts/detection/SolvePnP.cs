using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DlibDotNet;
using OpenCvSharp;
using UnityEngine;

namespace detection
{
    public struct SolvePnP
    {
        private static readonly Point3f[] ModelPoints = {
            new(0.0f, 0.03f, 0.11f),
            new(0.0f, -0.06f, 0.08f),
            new(-0.48f, 0.07f, 0.066f),
            new(0.48f, 0.07f, 0.066f),
            new(-0.03f, -0.007f, 0.088f),
            new(0.03f, -0.007f, 0.088f),
            new(-0.015f, 0.07f, 0.08f),
            new(0.015f, 0.07f, 0.08f),
        };

        private static readonly Mat ModelPointsMat = new(ModelPoints.Length, 1, MatType.CV_32FC3, ModelPoints);

        public static IEnumerable<Vector3> Solve(DlibDotNet.Point[] points, in Array2D<RgbPixel> image)
        {
            var imagePoints = new[]
            {
                new Point2f(points[30].X, points[30].Y),
                new Point2f(points[8].X, points[8].Y),
                new Point2f(points[45].X, points[45].Y),
                new Point2f(points[36].X, points[36].Y),
                new Point2f(points[54].X, points[54].Y),
                new Point2f(points[48].X, points[48].Y),
                new Point2f(points[42].X, points[42].Y),
                new Point2f(points[39].X, points[39].Y),
            };
            
            var imagePointsMat = new Mat(imagePoints.Length, 1, MatType.CV_32FC2, imagePoints);
            var distCoeffs = new Mat(4, 1, MatType.CV_64FC1, 0);
            var focalLength = image.Columns;
            var center = new Point2d(image.Columns / 2.0, image.Rows / 2.0);
            var cameraMatrix = new[,]{{focalLength, 0, center.X}, {0, focalLength, center.Y}, {0, 0, 1}};
            var cameraMatrixMat = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix);
            
            var rVec = new Mat();
            var tVec = new Mat();
            Cv2.SolvePnP(ModelPointsMat, 
                imagePointsMat, 
                cameraMatrixMat, 
                distCoeffs, 
                rVec, 
                tVec);
            
            var projMatrixMat = new Mat();
            var posDouble = new double[3];
            var proj = new double[9];
            Debug.Log($"{rVec.Rows}, {rVec.Height}");
            
            Marshal.Copy(tVec.Data, posDouble, 0, 3);
            Cv2.Rodrigues(rVec, projMatrixMat);
            Marshal.Copy(projMatrixMat.Data, proj, 0, 9);

            var objPosition = new Vector3((float)posDouble[0], (float)posDouble[1], (float)posDouble[2]);
            var objRotation = RotMatToQuaternion(proj).eulerAngles;
            
            return new[] {objPosition, objRotation};
        }

        private static Quaternion RotMatToQuaternion(in double[] projmat)
        {
            var quaternion = new Quaternion();
            var elem = new double[4];
            elem[0] = projmat[0] - projmat[4] - projmat[8] + 1.0f;
            elem[1] = -projmat[0] + projmat[4] - projmat[8] + 1.0f;
            elem[2] = -projmat[0] - projmat[4] + projmat[8] + 1.0f;
            elem[3] = projmat[0] + projmat[4] + projmat[8] + 1.0f;
            
            uint biggestIndex = 0;
            for (uint i = 1; i < 4; i++)
            {
                if(elem[i] > elem[biggestIndex])
                    biggestIndex = i;
            }
            
            if(elem[biggestIndex] < 0.0f)
                return quaternion;
            
            var v = (float)Math.Sqrt(elem[biggestIndex]) * 0.5f;
            var mult = 0.25f / v;

            switch (biggestIndex)
            {
                case 0:
                    quaternion.x = v;
                    quaternion.y = (float)(projmat[1] + projmat[3]) * mult;
                    quaternion.z = (float)(projmat[2] + projmat[6]) * mult;
                    quaternion.w = (float)(projmat[5] - projmat[7]) * mult;
                    break;
                
                case 1:
                    quaternion.x = (float)(projmat[1] + projmat[3]) * mult;
                    quaternion.y = v;
                    quaternion.z = (float)(projmat[5] + projmat[7]) * mult;
                    quaternion.w = (float)(projmat[6] - projmat[2]) * mult;
                    break;
                
                case 2:
                    quaternion.x = (float)(projmat[2] + projmat[6]) * mult;
                    quaternion.y = (float)(projmat[5] + projmat[7]) * mult;
                    quaternion.z = v;
                    quaternion.w = (float)(projmat[1] - projmat[3]) * mult;
                    break;
                
                case 3:
                    quaternion.x = (float)(projmat[5] - projmat[7]) * mult;
                    quaternion.y = (float)(projmat[6] - projmat[2]) * mult;
                    quaternion.z = (float)(projmat[1] - projmat[3]) * mult;
                    quaternion.w = v;
                    break;
            }
            
            return quaternion;
        }
    }
}
