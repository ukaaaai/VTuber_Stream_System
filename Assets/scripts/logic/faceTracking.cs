using UnityEngine;
using OpenCvSharp;
using DlibDotNet;
using System.Runtime.InteropServices;

public class faceTracking
{
    private faceTracking() { }
    private static faceTracking _instance = new faceTracking();
    public static faceTracking getInstance() => _instance;

    public int cameraID = 0;

    public void tracking()
    {
        //Matオブジェクトにwebカメラ画像を読み込み
        Mat mat = new Mat();
        VideoCapture capture = new VideoCapture();
        capture.Open(cameraID);
        capture.Read(mat);

        //画像のリサイズ
        Mat output = new Mat();
        Cv2.Resize(mat, output, new Size(320, 180));
        mat.Dispose();

        //DlibDotNetに画像を読むこむ
        byte[] array = new byte[output.Width * output.Height * output.ElemSize()];
        Marshal.Copy(output.Data, array, 0, array.Length);

        Array2D<RgbPixel> image = Dlib.LoadImageData<RgbPixel>(array, (uint)output.Height, (uint)output.Width, (uint)(output.Width * output.ElemSize()));
        output.Dispose();

        //顔検出
        FrontalFaceDetector detector = Dlib.GetFrontalFaceDetector();
        Rectangle[] rectangles = detector.Operator(image);


    }
    
}
