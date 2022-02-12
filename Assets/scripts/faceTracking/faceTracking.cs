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

        videoCapture = new VideoCapture(0);

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

        using (FullObjectDetection shapes = shape.Detect(image, rectangles[0]))
        {
            for(uint i = 0; i < points.Length; i++)
            {
                points[i] = shapes.GetPart(i);
            }
        }
    }
}
