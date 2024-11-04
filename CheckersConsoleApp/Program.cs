using System;

using CheckersLogic.Models;  
// is available?

namespace CheckersConsoleApp
{
    class Program
    {
        // The entry point of the application  (must be static?)
        static void Main(string[] args)
        {
            Game game = new Game();
            //Основной цикл игры. Запускается, пока не завершена игра (!game.IsGameOver()).
            //В каждой итерации показывается доска и запрашивается ход у пользователя.
            //Если ход успешен, игра продолжается. Если нет, просим пользователя ввести ход заново.
            //Когда игра заканчивается, выводится победитель
            while (!game.IsGameOver())
            {
                PrintBoard(game.GetBoard());
                Console.WriteLine(game.IsPlayerOneTurn() ? "Player 1's turn" : "Player 2's turn");

                var (startRow, startCol, endRow, endCol) = GetMoveFromUser();

                if (game.MakeMove(startRow, startCol, endRow, endCol))
                {
                    Console.WriteLine("Move successful!");
                }
                else
                {
                    Console.WriteLine("Invalid move, try again.");
                }
            }

            Console.WriteLine("Game Over!");
        }
        //Запрашивает ход от пользователя, в формате "A2 B3".
        //Преобразует введённые буквы и цифры в индексы массива для работы с доской.
        //Проверяет корректность ввода и диапазон значений.
        static (int, int, int, int) GetMoveFromUser()
        {
            while (true)
            {
                Console.Write("Enter move (e.g., 'A2 B3'): ");
                string? moveInput = Console.ReadLine()?.ToUpper(); // Преобразуем ввод в верхний регистр для удобства

                if (string.IsNullOrEmpty(moveInput))
                {
                    Console.WriteLine("No input provided. Please enter a valid move.");
                    continue;
                }

                string[] moveParts = moveInput.Split(' ');

                if (moveParts.Length != 2 || moveParts[0].Length != 2 || moveParts[1].Length != 2)
                {
                    Console.WriteLine("Invalid format. Please use the format 'A2 B3'.");
                    continue;
                }

                try
                {
                    // Преобразуем буквы (A-H) в индексы столбцов (0-7)
                    int startCol = moveParts[0][0] - 'A';
                    int startRow = int.Parse(moveParts[0][1].ToString()) - 1; // Преобразуем строки в индексы (1-8 -> 0-7)
                    int endCol = moveParts[1][0] - 'A';
                    int endRow = int.Parse(moveParts[1][1].ToString()) - 1;

                    // Проверяем диапазоны допустимых значений
                    if (startRow < 0 || startRow > 7 || startCol < 0 || startCol > 7 ||
                        endRow < 0 || endRow > 7 || endCol < 0 || endCol > 7)
                    {
                        Console.WriteLine("Coordinates out of bounds. Please try again.");
                        continue;
                    }

                    return (startRow, startCol, endRow, endCol);
                }
                catch
                {
                    Console.WriteLine("Invalid input. Please enter valid coordinates.");
                }
            }
        }

        //Отображает текущую доску. Пустые клетки отображаются как точки или символы "░" в зависимости от их цвета.
        //Фишки игрока 1 отображаются как X(для обычной) и K(для дамки).
        static void PrintBoard(Board board)
{
    Console.Clear();
    Console.WriteLine("  A B C D E F G H");
    for (int row = 0; row < Board.BoardSize; row++)
    {
        Console.Write($"{row + 1} ");
        for (int col = 0; col < Board.BoardSize; col++)
        {
            var piece = board.Grid[row, col];
            if (piece == null)
            {
                // Простое отображение для пустых клеток
                Console.Write(((row + col) % 2 == 0) ? ". " : "░ ");
            }
            else if (piece.IsPlayerOne)
            {
                // Фишки игрока 1: X для пешек, K для королей
                Console.Write(piece.Type == PieceType.King ? "K " : "X ");
            }
            else
            {
                // Фишки игрока 2: O для пешек, Q для королей
                Console.Write(piece.Type == PieceType.King ? "Q " : "O ");
            }
        }
        Console.WriteLine();
    }
}
    }
}

