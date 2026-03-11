using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Domain;

namespace GameModules.TicTacToe.Application
{
    /// <summary>
    /// Управляет жизненным циклом одной игровой сессии.
    /// </summary>
    public interface ITicTacToeSessionController
    {
        bool IsRunning { get; }

        TicTacToeSessionSnapshot CurrentSnapshot { get; }

        event Action<TicTacToeSessionSnapshot> StateChanged;

        event Action<TicTacToeResult> Completed;

        void StartSession(TicTacToeLaunchOptions options);

        UniTask<bool> TryMakePlayerMoveAsync(
            BoardPosition position,
            CancellationToken cancellationToken = default);

        UniTask<TicTacToeResult> WaitForCompletionAsync(CancellationToken cancellationToken = default);

        void Cancel();
    }
}
