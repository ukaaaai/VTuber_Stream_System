using UnityEngine;
using Zenject;

namespace DI
{
    public class InstallerManager : MonoInstaller
    {
        [SerializeField]
        private TextAsset graph;
        
        public override void InstallBindings()
        {
            DetectorInstaller.Install(Container);
            MediaPipeGraphInstaller.Install(Container, graph);
        }
    }
}