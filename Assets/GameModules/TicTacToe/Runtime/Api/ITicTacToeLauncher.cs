using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameModules.TicTacToe.Api
{
    /// <summary>
    /// Узкий внешний контракт для запуска миниигры из кода основной игры.
    /// </summary>
    public interface ITicTacToeLauncher
    {
        UniTask<TicTacToeResult> LaunchAsync(
            TicTacToeLaunchOptions options,
            CancellationToken cancellationToken = default);
    }
}
