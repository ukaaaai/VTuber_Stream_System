using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

namespace UI
{
    public sealed class PreviewMat : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        
        public void Preview(Mat mat)
        {
            var width = mat.Width;
            var height = mat.Height;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGRA2RGB);
            var bytes = new byte[mat.Total() * mat.Channels()];
            Marshal.Copy(mat.Data, bytes, 0, bytes.Length);
            texture.LoadRawTextureData(bytes);
            texture.Apply();
            rawImage.texture = texture;
        }
    }
}
