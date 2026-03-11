using GameModules.TicTacToe.Application;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Регистрирует прикладные сервисы и фабрики игровых сессий.
    /// </summary>
    public sealed class TicTacToeApplicationInstaller : Installer<TicTacToeApplicationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindFactory<TicTacToeSessionController, TicTacToeSessionController.Factory>();
        }
    }
}
