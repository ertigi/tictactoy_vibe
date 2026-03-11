using System;
using GameModules.TicTacToe.Config;
using GameModules.TicTacToe.Installers;
using UnityEngine;
using Zenject;

namespace GameModules.TicTacToe.Demo
{
    /// <summary>
    /// Устанавливает зависимости demo-сцены и подключает модуль миниигры.
    /// </summary>
    public sealed class TicTacToeDemoInstaller : MonoInstaller
    {
        [SerializeField]
        private TicTacToeModuleSettings moduleSettings;

        public override void InstallBindings()
        {
            if (moduleSettings == null)
            {
                throw new InvalidOperationException("TicTacToeModuleSettings is not assigned on demo installer.");
            }

            TicTacToeModuleBindings.Install(Container, moduleSettings);
            Container.Bind<TicTacToeDemoCaller>().AsSingle();
        }
    }
}
