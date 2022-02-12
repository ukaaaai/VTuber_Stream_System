using UnityEngine;
using System;
using DlibDotNet;
using OpenCvSharp;
using System.Runtime.InteropServices;

public class faceTracking : MonoBehaviour
{
    [SerializeField] static int width = 1920;
    [SerializeField] static int height = 1080;
    [SerializeField] static int fps = 60;
    private static WebCamTexture webCamTexture;
    private static WebCamDevice webCamDevice;

    private static VideoCapture videoCapture;
    private faceTracking() { }

    public static void trackingStart()
    {

    }

    public static void changeDevice(WebCamDevice device)
    {
        /*
        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
        WebCamDevice deviceTmp = webCamDevice;
        webCamDevice = device;
        try
        {
            webCamTexture = new WebCamTexture(webCamDevice.name, width, height, fps);
        }
        catch (Exception)
        {
            webCamDevice = deviceTmp;
            webCamTexture = new WebCamTexture(webCamTexture.name, width, height, fps);

            throw new Exception("change faild");
        }
        */
        webCamDevice = device;

        videoCapture = new VideoCapture(webCamDevice.name);

        videoCapture.FrameWidth = width;
        videoCapture.FrameHeight = height;
        videoCapture.Fps = fps;
    }

    private void Tracking()
    {
        if (!videoCapture.IsOpened())
        {
            return;
        }
        Mat mat = new Mat();
        videoCapture.Read(mat);
        Mat resized = new Mat();

        Cv2.Resize(mat, resized, new Size(320, 180));
        mat.Release();

        byte[] array = new byte[resized.Width * resized.Height * resized.ElemSize()];
        Marshal.Copy(resized.Data, array, 0, array.Length);

        Array2D<RgbPixel> image = Dlib.LoadImageData<RgbPixel>(array, (uint)resized.Height, (uint)resized.Width, (uint)(resized.Width * resized.ElemSize()));

        FrontalFaceDetector detector = Dlib.GetFrontalFaceDetector();
        Rectangle[] rectangles = detector.Operator(image);

        ShapePredictor shape = ShapePredictor.Deserialize("");

        DlibDotNet.Point[] points = new DlibDotNet.Point[68];

        if(rectangles[0] == null)
        {
            throw new Exception("data is empty");
        }

        using (FullObjectDetection shapes = shape.Detect(image, rectangles[0]))
        {
            for(uint i = 0; i < points.Length; i++)
            {
                points[i] = shapes.GetPart(i);
            }
        }

        Point3f[] model_points = new Point3f[8];
        model_points[0] = new Point3f(0.0f, 0.03f, 0.11f);
        model_points[1] = new Point3f(0.0f, -0.06f, 0.08f);
        model_points[2] = new Point3f(-0.048f, 0.07f, 0.066f);
        model_points[3] = new Point3f(0.048f, 0.07f, 0.066f);
        model_points[4] = new Point3f(-0.03f, -0.007f, 0.088f);
        model_points[5] = new Point3f(0.03f, -0.007f, 0.088f);
        model_points[6] = new Point3f(-0.015f, 0.07f, 0.08f);
        model_points[7] = new Point3f(0.015f, 0.07f, 0.08f);

        Mat model_points_mat = new Mat(model_points.Length, 1, MatType.CV_32FC3, model_points);

        Point2f[] image_points = new Point2f[8];
        image_points[0] = new Point2f(points[30].X, points[30].Y);
        image_points[1] = new Point2f(points[8].X, points[8].Y);
        image_points[2] = new Point2f(points[45].X, points[45].Y);
        image_points[3] = new Point2f(points[36].X, points[36].Y);
        image_points[4] = new Point2f(points[54].X, points[54].Y);
        image_points[5] = new Point2f(points[48].X, points[48].Y);
        image_points[6] = new Point2f(points[42].X, points[42].Y);
        image_points[7] = new Point2f(points[39].X, points[39].Y);

        Mat image_points_mat = new Mat(image_points.Length, 1, MatType.CV_32FC2, image_points);

        Mat dist_coeffs_mat = new Mat(4, 1, MatType.CV_64FC1, 0);
        int focal_length = image.Columns;
        Point2d center = new Point2d(image.Columns / 2, image.Rows / 2);
        double[,] camera_matrix = new double[3, 3] { { focal_length, 0, center.X }, { 0, focal_length, center.Y }, { 0, 0, 1 } };
        Mat camera_matrix_mat = new Mat(3, 3, MatType.CV_64FC1, camera_matrix);

        Mat rvec_mat = new Mat();
        Mat tvec_mat = new Mat();
        Cv2.SolvePnP(model_points_mat, image_points_mat, camera_matrix_mat, dist_coeffs_mat, rvec_mat, tvec_mat);

        Mat projMatrix_mat = new Mat();
        double[] pos_double = new double[3];
        double[] proj = new double[9];
        Marshal.Copy(tvec_mat.Data, pos_double, 0, 3);
        Cv2.Rodrigues(rvec_mat, projMatrix_mat);
        Marshal.Copy(projMatrix_mat.Data, proj, 0, 9);

        Vector3 obj_position = default;
        Vector3 obj_rotation = default;

        obj_position.x = -(float)pos_double[0];
        obj_position.y = (float)pos_double[1];
        obj_position.z = (float)pos_double[2];
        obj_rotation = RotMatToQuatanion(proj).eulerAngles;
    }

    private Quaternion RotMatToQuatanion(double[] projmat)
    {
        Quaternion quaternion = new Quaternion();
        double[] elem = new double[4];
        elem[0] = projmat[0] - projmat[4] - projmat[8] + 1.0f;
        elem[1] = -projmat[0] + projmat[4] - projmat[8] + 1.0f;
        elem[2] = -projmat[0] - projmat[4] + projmat[8] + 1.0f;
        elem[3] = projmat[0] + projmat[4] + projmat[8] + 1.0f;

        uint biggestIndex = 0;
        for(uint i = 0; i < 4; i++)
        {
            if(elem[i] > elem[biggestIndex])
            {
                biggestIndex = i;
            }
        }

        if(elem[biggestIndex] < 0.0f)
        {
            return quaternion;
        }

        float v = (float)Math.Sqrt(elem[biggestIndex] * 0.5f);
        float mult = 0.25f / v;

        switch (biggestIndex)
        {
            case 0:
                quaternion.x = v;
                quaternion.y = (float)(projmat[1] + projmat[3]) * mult;
                quaternion.z = (float)(projmat[6] + projmat[2]) * mult;
                quaternion.w = (float)(projmat[5] - projmat[7]) * mult;
                break;
            case 1:
                quaternion.x = (float)(projmat[1] + projmat[3]) * mult;
                quaternion.y = v;
                quaternion.z = (float)(projmat[5] + projmat[7]) * mult;
                quaternion.w = (float)(projmat[6] - projmat[2]) * mult;
                break;
            case 2:
                quaternion.x = (float)(projmat[6] + projmat[2]) * mult;
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
