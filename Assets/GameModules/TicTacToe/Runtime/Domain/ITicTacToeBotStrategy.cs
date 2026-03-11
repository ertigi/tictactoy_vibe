namespace GameModules.TicTacToe.Domain
{
    /// <summary>
    /// Точка расширения для логики бота.
    /// </summary>
    public interface ITicTacToeBotStrategy
    {
        bool TryChooseMove(
            TicTacToeBoard board,
            BoardMark botMark,
            BoardMark playerMark,
            out BoardPosition position);
    }
}
