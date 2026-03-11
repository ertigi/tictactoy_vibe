using System;
using GameModules.TicTacToe.Config;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Единая точка регистрации зависимостей модуля в Zenject-контейнере.
    /// </summary>
    public static class TicTacToeModuleBindings
    {
        public static void Install(
            DiContainer container,
            TicTacToeModuleSettings settings)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            container.Bind<TicTacToeModuleSettings>().FromInstance(settings).AsSingle();

            TicTacToeDomainInstaller.Install(container);
            TicTacToeInfrastructureInstaller.Install(container);
            TicTacToePresentationInstaller.Install(container);
            TicTacToeApplicationInstaller.Install(container);
            TicTacToeApiInstaller.Install(container);
        }
    }
}
