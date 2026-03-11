using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Application;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Регистрирует публичный API модуля для внешнего кода.
    /// </summary>
    public sealed class TicTacToeApiInstaller : Installer<TicTacToeApiInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<TicTacToeModuleService>().AsSingle();
            Container.Bind<ITicTacToeModule>().To<TicTacToeModuleService>().FromResolve();
            Container.Bind<ITicTacToeLauncher>().To<TicTacToeModuleService>().FromResolve();
        }
    }
}
