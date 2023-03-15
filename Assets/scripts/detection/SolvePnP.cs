using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        public static Vector3[] Solve(in DlibDotNet.Point[] points, in int row, in int col)
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
            var center = new Point2d(col / 2.0, row / 2.0);
            var cameraMatrix = new[,]{{col, 0, center.X}, {0, col, center.Y}, {0, 0, 1}};
            var cameraMatrixMat = new Mat(3, 3, MatType.CV_64FC1, cameraMatrix);
            
            var rVec = new Mat();
            var tVec = new Mat();
            Cv2.SolvePnP(ModelPointsMat, 
                imagePointsMat, 
                cameraMatrixMat, 
                distCoeffs, 
                rVec, 
                tVec);
            
            Cv2.Rodrigues(rVec, rVec);
            
            var projMatrixMat = new Mat();
            Debug.Log("height" + rVec.Height + " width" + rVec.Width + " x" + rVec.At<float>(0) + " y" + rVec.At<float>(1) + " z:" + rVec.At<float>(2));
            var posDouble = new float[3];
            var proj = new float[9];
            
            Marshal.Copy(tVec.Data, posDouble, 0, 3);
            Cv2.Rodrigues(rVec, projMatrixMat);
            Marshal.Copy(projMatrixMat.Data, proj, 0, 9);

            IEnumerable<Mat> projMat = new[] { projMatrixMat.Clone(), tVec.Clone() };
            Cv2.HConcat(projMat, projMatrixMat);

            Cv2.DecomposeProjectionMatrix(projMatrixMat, cameraMatrixMat, rVec, tVec);

            Cv2.Rodrigues(rVec, rVec);
            var rotVecs = new Vector3[]
            {
                new() { x = rVec.At<float>(0, 0), y = rVec.At<float>(1, 0), z = rVec.At<float>(2, 0) },
                new() { x = rVec.At<float>(0, 1), y = rVec.At<float>(1, 1), z = rVec.At<float>(2, 1) },
                new() { x = rVec.At<float>(0, 2), y = rVec.At<float>(1, 2), z = rVec.At<float>(2, 2) }
            };
            
            var objRotation = new Vector3(rotVecs[0].x, rotVecs[1].y, rotVecs[2].z);


            var objPosition = new Vector3(posDouble[0], posDouble[1], posDouble[2]);
            
            /*var roll = (float)Math.Atan2(proj[5], proj[8]);
            var pitch = (float)Math.Asin(proj[2]);
            var yaw = (float)Math.Atan2(proj[1], proj[0]);
            var objRotation = new Vector3(roll, pitch, yaw);//*/

            return new[] {objPosition, objRotation};
        }

        private static Vector3 RotMatToEulerAngles(in float[] projmat)
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
                return quaternion.normalized.eulerAngles;
            
            var v = (float)Math.Sqrt(elem[biggestIndex]) * 0.5f;
            var mult = 0.25f / v;

            switch (biggestIndex)
            {
                case 0:
                    quaternion.x = v;
                    quaternion.y = (projmat[1] + projmat[3]) * mult;
                    quaternion.z = (projmat[2] + projmat[6]) * mult;
                    quaternion.w = (projmat[5] - projmat[7]) * mult;
                    break;
                
                case 1:
                    quaternion.x = (projmat[1] + projmat[3]) * mult;
                    quaternion.y = v;
                    quaternion.z = (projmat[5] + projmat[7]) * mult;
                    quaternion.w = (projmat[6] - projmat[2]) * mult;
                    break;
                
                case 2:
                    quaternion.x = (projmat[2] + projmat[6]) * mult;
                    quaternion.y = (projmat[5] + projmat[7]) * mult;
                    quaternion.z = v;
                    quaternion.w = (projmat[1] - projmat[3]) * mult;
                    break;
                
                case 3:
                    quaternion.x = (projmat[5] - projmat[7]) * mult;
                    quaternion.y = (projmat[6] - projmat[2]) * mult;
                    quaternion.z = (projmat[1] - projmat[3]) * mult;
                    quaternion.w = v;
                    break;
            }
            
            return quaternion.normalized.eulerAngles;
        }
    }
}
