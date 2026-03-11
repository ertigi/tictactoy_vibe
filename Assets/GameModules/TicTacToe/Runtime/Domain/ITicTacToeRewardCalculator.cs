using GameModules.TicTacToe.Api;

namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Точка расширения для расчета награды.
    /// </summary>
    public interface ITicTacToeRewardCalculator
    {
        TicTacToeRewardData Calculate(
            TicTacToeRoundOutcome outcome,
            int emptyCellsCount,
            TicTacToeRewardOverrides overrides = null);
    }
}
