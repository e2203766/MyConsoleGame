using System;

namespace CheckersLogic.Models
{
    public class Board
    {
        public const int BoardSize = 8;  // Размер доски
        public Piece?[,] Grid { get; private set; }  // Сетка с nullable объектами Piece

        public Board()
        {
            Grid = new Piece?[BoardSize, BoardSize];  // Инициализация доски
            InitializeBoard();  // Заполнение доски фишками
        }

        // Метод для инициализации доски
        private void InitializeBoard()
        {
            // Инициализация фишек игрока 1 (верхние 3 ряда)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if ((row + col) % 2 == 1)  // Размещение фишек на черных клетках
                        Grid[row, col] = new Piece(true);  // true - фишки игрока 1
                }
            }

            // Инициализация фишек игрока 2 (нижние 3 ряда)
            for (int row = 5; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if ((row + col) % 2 == 1)  // Размещение фишек на черных клетках
                        Grid[row, col] = new Piece(false);  // false - фишки игрока 2
                }
            }
        }

        // Проверка корректности хода
        public bool IsValidMove(int startRow, int startCol, int endRow, int endCol, bool isPlayerOne)
        {
            var piece = Grid[startRow, startCol];

            // Проверка наличия фишки игрока
            if (piece == null || piece.IsPlayerOne != isPlayerOne)
                return false;

            // Клетка назначения должна быть пустой
            if (Grid[endRow, endCol] != null)
                return false;

            int rowDiff = Math.Abs(endRow - startRow);
            int colDiff = Math.Abs(endCol - startCol);

            // Логика для обычных фишек
            if (!piece.IsKing)
            {
                // Обычный ход (одна клетка по диагонали)
                if (rowDiff == 1 && colDiff == 1)
                {
                    if ((isPlayerOne && endRow > startRow) || (!isPlayerOne && endRow < startRow))
                        return true;
                }

                // Захват (две клетки по диагонали)
                if (rowDiff == 2 && colDiff == 2)
                {
                    int middleRow = (startRow + endRow) / 2;
                    int middleCol = (startCol + endCol) / 2;

                    // Проверка наличия фишки противника для захвата
                    var middlePiece = Grid[middleRow, middleCol];
                    if (middlePiece != null && middlePiece.IsPlayerOne != isPlayerOne)
                        return true;
                }
            }
            // Логика для дамки
            else
            {
                // Дамка может двигаться на любое количество клеток по диагонали
                if (rowDiff == colDiff)
                {
                    int rowStep = (endRow > startRow) ? 1 : -1;
                    int colStep = (endCol > startCol) ? 1 : -1;

                    // Проверяем, что путь свободен
                    for (int i = 1; i < rowDiff; i++)
                    {
                        if (Grid[startRow + i * rowStep, startCol + i * colStep] != null)
                            return false;  // Если путь не свободен
                    }

                    // Проверка на захват для дамки
                    if (rowDiff > 1)
                    {
                        int middleRow = (startRow + endRow) / 2;
                        int middleCol = (startCol + endCol) / 2;
                        var middlePiece = Grid[middleRow, middleCol];

                        if (middlePiece != null && middlePiece.IsPlayerOne != isPlayerOne)
                            return true; // Захват возможен
                    }

                    return true;  // Если путь свободен, дамка может двигаться
                }
            }

            return false;
        }



        // Метод для перемещения фишки
        public void MovePiece(int startRow, int startCol, int endRow, int endCol)
        {
            var piece = Grid[startRow, startCol];
            Grid[endRow, endCol] = piece;
            Grid[startRow, startCol] = null;  // Освобождение начальной клетки

            // Превращение в короля
            if (piece?.IsPlayerOne == true && endRow == BoardSize - 1)
                piece.PromoteToKing();
            else if (piece?.IsPlayerOne == false && endRow == 0)
                piece.PromoteToKing();

            // Проверка на захват (если шаг был на 2 клетки или больше для дамки)
            if (Math.Abs(endRow - startRow) > 1)
            {
                int rowStep = (endRow > startRow) ? 1 : -1;
                int colStep = (endCol > startCol) ? 1 : -1;

                // Если дамка или обычная фишка прыгает через фишку противника
                for (int i = 1; i < Math.Abs(endRow - startRow); i++)
                {
                    int middleRow = startRow + i * rowStep;
                    int middleCol = startCol + i * colStep;
                    var middlePiece = Grid[middleRow, middleCol];

                    // Удаление захваченной фишки противника
                    if (middlePiece != null && middlePiece.IsPlayerOne != piece?.IsPlayerOne)
                    {
                        Grid[middleRow, middleCol] = null;  // Захваченная фишка удалена
                        break;
                    }
                }
            }
        }



        // Проверка, может ли игрок сделать ход
        public bool PlayerHasMoves(bool isPlayerOne)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var piece = Grid[row, col];
                    if (piece != null && piece.IsPlayerOne == isPlayerOne)
                    {
                        if (CanMoveOrCapture(row, col, isPlayerOne))
                            return true;
                    }
                }
            }
            return false;
        }

        // Проверка возможности хода или захвата для фишки
        private bool CanMoveOrCapture(int row, int col, bool isPlayerOne)
        {
            // Смещения для проверки соседних клеток и возможных захватов
            int[] rowOffsets = { -1, 1, -2, 2 };
            int[] colOffsets = { -1, 1, -2, 2 };

            foreach (var rowOffset in rowOffsets)
            {
                foreach (var colOffset in colOffsets)
                {
                    int newRow = row + rowOffset;
                    int newCol = col + colOffset;

                    // Проверяем, что новая позиция в пределах доски
                    if (newRow >= 0 && newRow < BoardSize && newCol >= 0 && newCol < BoardSize)
                    {
                        // Проверяем, может ли фишка сделать ход или захват на новую позицию
                        if (IsValidMove(row, col, newRow, newCol, isPlayerOne))
                            return true;
                    }
                }
            }
            return false;
        }

        // Проверка, есть ли у игрока хотя бы одна фишка на доске
        public bool HasPieces(bool isPlayerOne)
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var piece = Grid[row, col];
                    if (piece != null && piece.IsPlayerOne == isPlayerOne)
                        return true;  // Игрок имеет хотя бы одну фишку
                }
            }
            return false;  // Фишек нет
        }

        // Метод для проверки, может ли игрок выполнить захват
        public bool CanCapture(int row, int col, bool isPlayerOne)
        {
            // Смещения для возможных захватов (прыжков на 2 клетки)
            int[] rowOffsets = { -2, 2 };
            int[] colOffsets = { -2, 2 };

            foreach (var rowOffset in rowOffsets)
            {
                foreach (var colOffset in colOffsets)
                {
                    int newRow = row + rowOffset;
                    int newCol = col + colOffset;

                    // Проверяем, что новая позиция в пределах доски
                    if (newRow >= 0 && newRow < BoardSize && newCol >= 0 && newCol < BoardSize)
                    {
                        // Если захват возможен, возвращаем true
                        if (IsValidMove(row, col, newRow, newCol, isPlayerOne))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;  // Захват невозможен
        }
    }
}
