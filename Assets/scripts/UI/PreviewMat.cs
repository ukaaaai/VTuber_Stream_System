using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

namespace UI
{
    public class PreviewMat : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        
        public void Preview(Mat mat)
        {
            var width = mat.Width;
            var height = mat.Height;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var c = new Color32[height * width];
            for(var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var color = mat.Get<Vec3b>(i, j);
                    c[j + i * width] = new Color32(color.Item2, color.Item1, color.Item0, 255);
                }
            }
            texture.SetPixels32(c);
            texture.Apply();
            rawImage.texture = texture;
        }
    }
}
