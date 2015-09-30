using System;
using System.Collections.Generic;
using GameClasses;

class TheGame
{
    static char[,] humanBoard;
    static char[,] aiBoard;

    static void Main()
    {
        Console.BufferHeight = Console.WindowHeight = 35;
        Console.BufferWidth = Console.WindowWidth = 55;

        //printing player/ai name
        Player player = new Player("Playerrrrrrrrrr");
        Console.SetCursorPosition(26 / 2-player.name.Length/2, 0);
        Console.WriteLine(player.Name);
        Player ai = new Player("Easy Bot");
        Console.SetCursorPosition(40, 0);
        Console.WriteLine(ai.Name);
        

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        List<Battleship> ships = new List<Battleship>();

        humanBoard = player.board;
        aiBoard = ai.Board;
        PrintMatrix(player.Board);
        
        PrintAIMatrix(player.Board);

        string[] shipRanks = { "Scout" ,"Submarine", "Destroyer", "BattleShip", "Aircraft Carrier"};

        for (int i = 0; i < 5; i++)
        {
            ships.Add(MakeShipAI(shipRanks[i], shipSizes[i]));
        }

        for (int i = 0; i < 2; i++)
        {
            Console.SetCursorPosition(0,14);
            ships.Add(MakeShip(shipRanks[i], shipSizes[i]));
            DrawShip(ships[i]);
            Console.Clear();
            PrintMatrix(player.Board);
            PrintAIMatrix(player.Board);
        }
        
        //player.Board = humanBoard;

        
    }

    static Battleship MakeShip(string rank, int size)
    {
        Console.WriteLine("Battleship Rank: {0}, Size: {1}", rank, size);
        Console.Write("Input X: ");
        int coordinatesX = int.Parse(Console.ReadLine());
        Console.Write("Input Y: ");
        int coordinatesY = int.Parse(Console.ReadLine());
        Console.Write("Input Direction: ");
        char direction = Convert.ToChar(Console.ReadLine());

        bool validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, humanBoard);
        while (!validPosition)
        {
            Console.WriteLine("Invalid input.. Try again");
            Console.Write("Input X: ");
            coordinatesX = int.Parse(Console.ReadLine());
            Console.Write("Input Y: ");
            coordinatesY = int.Parse(Console.ReadLine());
            Console.Write("Input Direction: ");
            direction = Convert.ToChar(Console.ReadLine());
            validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, humanBoard);
        }
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction);
        return ship;
    }
    static Battleship MakeShipAI(string rank, int size)
    {
        Random rnd = new Random();
        int coordinatesX = rnd.Next(0, 10);
        int coordinatesY = rnd.Next(0, 10);
        int intDirection = rnd.Next(1, 5);
        char direction = ' ';
        switch (intDirection)
        {
            case 1: direction = 'R'; break;
            case 2: direction = 'D'; break;
            case 3: direction = 'L'; break;
            case 4: direction = 'U'; break;
            default: break;
        }
        bool validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, humanBoard);
        while (!validPosition)
        {
            coordinatesX = rnd.Next(0, 10);
            coordinatesY = rnd.Next(0, 10);
            intDirection = rnd.Next(1, 5);
            switch (intDirection)
            {
                case 1: direction = 'R'; break;
                case 2: direction = 'D'; break;
                case 3: direction = 'L'; break;
                case 4: direction = 'U'; break;
                default: break;
            }
            validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, humanBoard);
        }
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction);
        return ship;
    }
    static void PrintMatrix(char[,] matrix)
    {
        Console.WriteLine("    A B C D E F G H I J");
        Console.WriteLine("    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("{0} |", i);
                }
                if (matrix[i, j] != 'O')
                {
                    Console.Write("{0}".PadLeft(4), matrix[i, j]);
                }
                else
                {
                    Console.Write("{0}".PadLeft(4), ".");
                }
            }
            Console.WriteLine();
        }
    }
    static bool ValidatePosition(int shipX, int shipY, int shipLength, char shipDirection, char[,] matrix)
    {
        bool shipPlaced = true;

        for (int i = 0; i < shipLength; i++)
        {
            if (shipX < 0 || shipY < 0 || shipX >= matrix.GetLength(0) || shipY >= matrix.GetLength(1) || matrix[shipX, shipY] != 'O')
            {
                shipPlaced = false;
                break;
            }
            switch (shipDirection)
            {
                case 'R': shipY++; break;
                case 'D': shipX++; break;
                case 'L': shipY--; break;
                case 'U': shipX--; break;
                default: break;
            }
        }
        return shipPlaced;
    }
    static void DrawShip(Battleship ship)
    {
        for (int i = 0; i < ship.Size; i++)
        {
            humanBoard[ship.CurrentX, ship.CurrentY] = 'S';
            switch (ship.Direction)
            {
                case 'R': ship.y += 1; break;
                case 'D': ship.x += 1; break;
                case 'L': ship.y -= 1; break;
                case 'U': ship.x -= 1; break;
                default: break;
            }
        }
    }
    static void PrintAIMatrix(char[,] matrix)
    {
        Console.SetCursorPosition(30, 1);
        Console.WriteLine("    A B C D E F G H I J");
        Console.SetCursorPosition(30, 2);
        Console.WriteLine("    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            Console.SetCursorPosition(30, i+3);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("{0} |", i);
                }
                if (matrix[i, j] != 'O')
                {
                    Console.Write("{0}".PadLeft(4), matrix[i, j]);
                }
                else
                {
                    Console.Write("{0}".PadLeft(4), ".");
                }
            }
            Console.WriteLine();
        }
    }
}