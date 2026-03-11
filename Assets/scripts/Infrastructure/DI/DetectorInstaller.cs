using detection;
using Zenject;

namespace Infrastructure.DI
{
    public class DetectorInstaller : Installer<DetectorInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IDetector>().To<DlibDetector>().AsSingle();
        }
    }
}