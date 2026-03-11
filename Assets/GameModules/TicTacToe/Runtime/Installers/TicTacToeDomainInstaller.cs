using GameModules.TicTacToe.Domain;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Регистрирует доменную логику матча, AI и расчет награды.
    /// </summary>
    public sealed class TicTacToeDomainInstaller : Installer<TicTacToeDomainInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<TicTacToeRules>().AsSingle();
            Container.Bind<ITicTacToeBotStrategy>().To<SimpleTicTacToeBotStrategy>().AsSingle();
            Container.Bind<ITicTacToeRewardCalculator>().To<DefaultTicTacToeRewardCalculator>().AsSingle();
        }
    }
}
