using GameModules.TicTacToe.Infrastructure;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Регистрирует инфраструктурные сервисы загрузки и создания UI.
    /// </summary>
    public sealed class TicTacToeInfrastructureInstaller : Installer<TicTacToeInfrastructureInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ITicTacToeScreenFactory>().To<TicTacToeScreenFactory>().AsSingle();
        }
    }
}
