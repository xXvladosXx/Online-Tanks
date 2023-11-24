using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Networking.Server;
using Zenject;

namespace Codebase.Runtime.Installers
{
    public class BoostrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TestPersistent>().AsSingle().NonLazy();
        }
    }
}