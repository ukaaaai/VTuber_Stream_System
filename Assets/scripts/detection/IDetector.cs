using Cysharp.Threading.Tasks;
using OpenCvSharp;

namespace detection
{
    public interface IDetector
    {
        public void Init();
        public Param? DetectTask(in Mat mat);
        
        public UniTask<Param?> Detect(in Mat mat)
        {
            return new UniTask<Param?>(DetectTask(mat));
        }
    }
}