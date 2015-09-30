﻿using System;
using System.Collections.Generic;
using GameClasses;

class TheGame
{
    static char[,] humanBoard;
    static char[,] aiBoard;

    static void Main()
    {
        // Console.SetBufferSize(0, 0);
        // Console.WindowHeight = 30;
        // Console.WindowWidth = 60;

        StartScreen();
        Player player = new Player("Some Player");
        Player ai = new Player("Easy Bot");

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        List<Battleship> ships = new List<Battleship>();

        humanBoard = player.board;
        aiBoard = ai.Board;

        string[] shipRanks = { "Scout" ,"Submarine", "Destroyer", "BattleShip", "Aircraft Carrier"};

        for (int i = 0; i < 1; i++)
        {
            ships.Add(MakeShip(shipRanks[i], shipSizes[i]));
            DrawShip(ships[i]);
            Console.Clear();
        }

        player.Board = humanBoard;

        PrintMatrix(player.Board);
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
}