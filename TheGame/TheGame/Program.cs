using System;

class TryPrintingBattleshipsMatirx
{
    static void Main(string[] args)
    {
        bool[,] matrix = new bool[10, 10];

        DrawShip(TryShipPosition(0, 0, 3, 'R', matrix), 0, 0, 3, 'R', matrix);
        PrintMatrix(matrix);
    }

    static void FillMatrix(bool[,] matrix)
    {

    }
    static void PrintMatrix(bool[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j])
                {
                    Console.Write("X".PadLeft(5));
                }
                else
                {
                    Console.Write("O".PadLeft(5));
                }
            }
            Console.WriteLine();
        }
    }
    static bool TryShipPosition(int shipX, int shipY, int shipLength, char shipDirection, bool[,] matrix)
    {
        bool shipPlaced = true;

        for (int i = 0; i < shipLength; i++)
        {
            if (shipX < 0 || shipY < 0 || shipX >= matrix.GetLength(0) || shipY >= matrix.GetLength(1) || matrix[shipX, shipY] == true)
            {
                shipPlaced = false;
                break;
            }
            switch (shipDirection)
            {
                case 'R':
                    shipY++; break;
                case 'D':
                    shipX++; break;
                case 'L':
                    shipY--; break;
                case 'U':
                    shipX--; break;
                default:
                    break;
            }
        }

        return shipPlaced;
    }
    static void DrawShip(bool draw, int shipX, int shipY, int shipLength, char shipDirection, bool[,] matrix)
    {
        if (draw)
        {
            for (int i = 0; i < shipLength; i++)
            {
                matrix[shipX, shipY] = true;
                switch (shipDirection)
                {
                    case 'R':
                        shipY++; break;
                    case 'D':
                        shipX++; break;
                    case 'L':
                        shipY--; break;
                    case 'U':
                        shipX--; break;
                    default:
                        break;
                }
            }
        }
    }
}