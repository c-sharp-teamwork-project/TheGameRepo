using System;
using System.Collections.Generic;

class TryPrintingBattleshipsMatirx
{
    struct Ship
    {
        public int shipRow;
        public int shipCol;
        public int shipLength;
        public string direction;

    }
    static void Main(string[] args)
    {
        //Console.SetBufferSize(0, 0);
        //Console.WindowHeight = 30;
        //Console.WindowWidth = 60;

        bool[,] humanBoard = new bool[5, 5];
        int[] shipSizes = { 2, 3, 3, 4, 5 };
        List<Ship> allTheShips = new List<Ship>();
        string[] shipNames = { "Destroyer", "Cruiser","Submarine", "BattleShip", "Aircraft Carrier"};
        for (int i = 0; i < shipSizes.Length; i++)
        {
            Console.WriteLine("Ship Name: {0}, Size: {1}", shipNames[i], shipSizes[i]);
            Ship cruiser = new Ship();
            Console.Write("Enter the ship's row: ");
            cruiser.shipRow = int.Parse(Console.ReadLine());
            Console.Write("Enter the ship's col: ");
            cruiser.shipCol = int.Parse(Console.ReadLine());
            
            cruiser.shipLength = shipSizes[i];

            Console.Write(@"Enter the ship's Direction (""Up"", ""Down"", ""Left"" or ""Right""): ");
            cruiser.direction = Console.ReadLine();

            while (!TryShipPosition(cruiser.shipRow, cruiser.shipCol, cruiser.shipLength, cruiser.direction, humanBoard))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input! Please input again: ");
                Console.WriteLine("Ship Name: {0}, Size: {1}", shipNames[i], shipSizes[i]);
                cruiser = new Ship();
                Console.Write("Enter the ship's row: ");
                cruiser.shipRow = int.Parse(Console.ReadLine());
                Console.Write("Enter the ship's col: ");
                cruiser.shipCol = int.Parse(Console.ReadLine());

                cruiser.shipLength = shipSizes[i];

                Console.Write(@"Enter the ship's Direction (""Up"", ""Down"", ""Left"" or ""Right""): ");
                cruiser.direction = Console.ReadLine();

            }
            DrawShip(true, cruiser.shipRow, cruiser.shipCol, cruiser.shipLength, cruiser.direction, humanBoard);
            Console.Clear();
        }

        PrintMatrix(humanBoard);
        //DrawShip(true, cruiser.shipRow, cruiser.shipCol, cruiser.shipLength, cruiser.direction, matrix);
       // PrintMatrix(matrix);
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
    static bool TryShipPosition(int shipX, int shipY, int shipLength, string shipDirection, bool[,] matrix)
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
                case "Right":
                    shipY++; break;
                case "Down":
                    shipX++; break;
                case "Left":
                    shipY--; break;
                case "Up":
                    shipX--; break;
                default:
                    break;
            }
        }

        return shipPlaced;
    }
    static void DrawShip(bool draw, int shipX, int shipY, int shipLength, string shipDirection, bool[,] matrix)
    {
        if (draw)
        {
            for (int i = 0; i < shipLength; i++)
            {
                matrix[shipX, shipY] = true;
                switch (shipDirection)
                {
                    case "Right":
                        shipY++; break;
                    case "Down":
                        shipX++; break;
                    case "Left":
                        shipY--; break;
                    case "Up":
                        shipX--; break;
                    default:
                        break;
                }
            }
        }
    }
}