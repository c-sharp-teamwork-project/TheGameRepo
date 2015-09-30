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

        StartScreen();
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
        PrintMatrix(player.Board, player.name);
        
        PrintAIMatrix(player.Board, ai.name);

        string[] shipRanks = { "Scout" ,"Submarine", "Destroyer", "BattleShip", "Aircraft Carrier"};

        // create a separate list for The AI

        //for (int i = 0; i < 5; i++)
        //{
        //    ships.Add(MakeShip(shipRanks[i], shipSizes[i]));
        //}

        for (int i = 0; i < 5; i++)
        {

            Console.SetCursorPosition(1, 14);
            ships.Add(MakeShip(shipRanks[i], shipSizes[i]));
            DrawShip(ships[i]);
            
        
            Console.Clear();
            
            PrintMatrix(player.Board, player.name);
            PrintAIMatrix(player.Board, ai.name);
        }
        
        //player.Board = humanBoard;

        
    }

    static Battleship MakeShip(string rank, int size)
    {
        Console.WriteLine("Battleship Rank: {0}, Size: {1}", rank, size);
        Console.Write("Input row and col coordinates: ");
        string[] coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        int coordinatesX = ConvertCoordinateToInt(coordinates[0]);
        int coordinatesY = int.Parse(coordinates[1]) - 1;
        Console.Write("Input Direction: ");
        char direction = Convert.ToChar(Console.ReadLine());

        bool validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, humanBoard);
        while (!validPosition)
        {
            Console.WriteLine("Invalid input.. Try again");
            Console.Write("Input row and col coordinates: ");
            coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            coordinatesX = ConvertCoordinateToInt(coordinates[0]);
            coordinatesY = int.Parse(coordinates[1]) - 1;
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
    static void PrintMatrix(char[,] matrix, string playerName)
    {
        //name printing
        Console.SetCursorPosition(26 / 2 - playerName.Length / 2, 0);
        Console.WriteLine(playerName);
        //name printing

        Console.WriteLine("    1 2 3 4 5 6 7 8 9 10");
        Console.WriteLine("    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("{0} |", GetRowChar(i));
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


    static void StartScreen()
    {
        Console.WriteLine("The Amazing Battleships");
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine();
        }
        Console.Write("Do you want to see the instuctions? (Y/N): ");
    Here:
        string agree = Console.ReadLine();
        if (agree == "Y")
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Instuctions: ");
            Console.WriteLine("1) In this game you will start by placing 5 ships on your board.");
            Console.Write(@"2) When asked for coordinates you have to input a letter from ""A"" to ""J"" (this is the row coordinate), followed by a number from 1 to 10 (this is the col coordinate) on a single line.");
            Console.WriteLine(@"3) When asked for a position you must enter ""L"" for ""Left"", ""R"" for ""Right"", ""U"" for ""Up"" or ""D"" for ""Down"".");
            Console.WriteLine("4) When the game starts the AI and you will take turns on bombarding your ships.");
            Console.WriteLine("5) The bombarding happens by inputting the coordinates that you want to bombard (the same as with placing the ships.");
            Console.WriteLine("6) Whoever sinks all the ships first is the winner.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue: ");
            Console.ReadLine();
            Console.Clear();
        }
        else if (agree == "N")
        {
            Console.Clear();
        }
        else
        {
            Console.WriteLine("Invalid input! Input again: ");
            goto Here;
        }
    }

    static void PrintAIMatrix(char[,] matrix, string aiName)

    {
        //name printing
        Console.SetCursorPosition(40, 0);
        Console.WriteLine(aiName);
        //name printing

        Console.SetCursorPosition(30, 1);
        Console.WriteLine("    1 2 3 4 5 6 7 8 9 10");
        Console.SetCursorPosition(30, 2);
        Console.WriteLine("    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            Console.SetCursorPosition(30, i+3);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("{0} |", GetRowChar(i));
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

    static char GetRowChar(int i)
    {
        char toReturn = ' ';
        switch (i)
        {
            case 0:
                toReturn = 'A';
                break;
            case 1:
                toReturn = 'B';
                break;
            case 2:
                toReturn = 'C';
                break;

            case 3:
                toReturn = 'D';
                break;
            case 4:
                toReturn = 'E';
                break;
            case 5:
                toReturn = 'F';
                break;

            case 6:
                toReturn = 'G';
                break;
            case 7:
                toReturn = 'H';
                break;
            case 8:
                toReturn = 'I';
                break;
            case 9:
                toReturn = 'J';
                break;
        }
        return toReturn;
    }

    static int ConvertCoordinateToInt(string input) 
    {
        int row = 0;
        switch (input)
        {
            case "A":
                row = 0;
                break;
            case "B":
                row = 1;
                break;
            case "C":
                row = 2;
                break;
            case "D":
                row = 3;
                break;
            case "E":
                row = 4;
                break;
            case "F":
                row = 5;
                break;
            case "G":
                row = 6;
                break;
            case "H":
                row = 7;
                break;
            case "I":
                row = 8;
                break;
            case "J":
                row = 9;
                break;
            default:
                break;
        }
        return row;
    }
}