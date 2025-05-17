using UnityEngine;
using Zenject;

namespace DI
{
    public class MediaPipeGraphInstaller :  Installer<TextAsset, MediaPipeGraphInstaller>
    {
        private readonly TextAsset _graph;
        
        public MediaPipeGraphInstaller(TextAsset graph)
        {
            _graph = graph;
        }
        
        public override void InstallBindings()
        {
            Container.Bind<TextAsset>().WithId("MediaPipeGraph").FromInstance(_graph).AsSingle();
        }
    }
}