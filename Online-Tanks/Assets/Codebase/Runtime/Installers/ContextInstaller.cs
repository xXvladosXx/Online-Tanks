using Zenject;

namespace Codebase.Runtime.Installers
{
    public class ContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TestContext>().AsSingle().NonLazy();
        }
    }
}