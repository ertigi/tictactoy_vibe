using System;
using GameModules.TicTacToe.Config;
using UnityEngine;
using Zenject;

namespace GameModules.TicTacToe.Installers
{
    /// <summary>
    /// Устанавливает все runtime-зависимости миниигры в контейнер.
    /// </summary>
    public sealed class TicTacToeModuleInstaller : MonoInstaller
    {
        [SerializeField]
        private TicTacToeModuleSettings settings;

        public override void InstallBindings()
        {
            if (settings == null)
            {
                throw new InvalidOperationException("TicTacToeModuleSettings is not assigned.");
            }

            TicTacToeModuleBindings.Install(Container, settings);
        }
    }
}
