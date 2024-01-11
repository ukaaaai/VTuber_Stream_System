using Zenject;

namespace DI
{
    public class InstallerManager : MonoInstaller
    {
        public override void InstallBindings()
        {
            DetectorInstaller.Install(Container);
        }
    }
}