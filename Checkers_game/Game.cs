using System;

namespace CheckersLogic.Models
{
    public class Game
    {
        private Board _board;
        private bool _isPlayerOneTurn;

        public Game()
        {
            _board = new Board();
            _isPlayerOneTurn = true;  // Игра начинается с хода игрока 1
        }

        // Проверка, закончилась ли игра для обоих игроков
        public bool IsGameOver()
        {
            // Игра завершена, если у обоих игроков нет фишек или ходов
            return !_board.PlayerHasMoves(_isPlayerOneTurn) || !_board.PlayerHasMoves(!_isPlayerOneTurn)
                || !_board.HasPieces(_isPlayerOneTurn) || !_board.HasPieces(!_isPlayerOneTurn);
        }

        // Метод для выполнения хода
        public bool MakeMove(int startRow, int startCol, int endRow, int endCol)
        {
            // Проверяем корректность хода
            if (_board.IsValidMove(startRow, startCol, endRow, endCol, _isPlayerOneTurn))
            {
                // Выполняем перемещение
                _board.MovePiece(startRow, startCol, endRow, endCol);

                // Проверка, был ли захват (если ход был на 2 клетки по диагонали)
                if (Math.Abs(endRow - startRow) == 2)
                {
                    // Если был захват, проверяем, можно ли продолжить захват
                    if (_board.CanCapture(endRow, endCol, _isPlayerOneTurn))
                    {
                        Console.WriteLine("You can continue capturing!");
                        return true;  // Продолжаем ход текущего игрока, не меняем ход
                    }
                }

                // Если захватов больше нет, передаем ход другому игроку
                _isPlayerOneTurn = !_isPlayerOneTurn;
                return true;
            }

            return false;
        }


        // Возвращает текущую доску
        public Board GetBoard() => _board;

        // Возвращает, ход какого игрока сейчас
        public bool IsPlayerOneTurn() => _isPlayerOneTurn;

        // Возвращает победителя, если игра закончена
        public string GetWinner()
        {
            if (!_board.HasPieces(true))  // Если у игрока 1 нет фишек
                return "Player 2 wins!";
            if (!_board.HasPieces(false))  // Если у игрока 2 нет фишек
                return "Player 1 wins!";

            // Если никто не может сделать ход
            if (!_board.PlayerHasMoves(true))
                return "Player 2 wins!";
            if (!_board.PlayerHasMoves(false))
                return "Player 1 wins!";

            return "No winner yet!";
        }
    }
}

