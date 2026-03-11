using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameModules.TicTacToe.Infrastructure
{
    /// <summary>
    /// Точка расширения для загрузки и инстанцирования UI.
    /// </summary>
    public interface ITicTacToeScreenFactory
    {
        UniTask PreloadAsync(CancellationToken cancellationToken = default);

        UniTask<TicTacToeScreenInstance> CreateAsync(
            Transform parent,
            CancellationToken cancellationToken = default);

        void ReleasePreloaded();
    }
}
