using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Публичный API модуля для внешнего кода.
    /// </summary>
    public interface ITicTacToeModule
    {
        bool IsRunning { get; }

        UniTask PreloadAsync(CancellationToken cancellationToken = default);

        UniTask<TicTacToeResult> RunAsync(
            TicTacToeLaunchOptions options,
            CancellationToken cancellationToken = default);

        void Release();
    }
}
