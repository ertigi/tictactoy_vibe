using GameModules.TicTacToe.Application;
using GameModules.TicTacToe.Presentation;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Регистрирует presentation-слой и фабрики presenter-объектов.
    /// </summary>
    public sealed class TicTacToePresentationInstaller : Installer<TicTacToePresentationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindFactory<TicTacToeScreenView, ITicTacToeSessionController, TicTacToePresenter, TicTacToePresenter.Factory>();
        }
    }
}
