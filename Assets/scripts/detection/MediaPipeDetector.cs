using OpenCvSharp;
using UnityEngine.TextCore.Text;
using Zenject;

namespace detection
{
    public class MediaPipeDetector : IDetector
    {
        [Inject(Id = "MediaPipeGraph")] private TextAsset _graph;

        public void Init()
        {
            
        }
        
        public Param? DetectTask(in Mat mat)
        {
            return null;
        }
    }
}