using System;
using System.Collections.Generic;
using GameClasses;
using System.Threading;

class TheGame
{
    static void Main()
    {
        Console.BufferHeight = Console.WindowHeight = 35;
        Console.BufferWidth = Console.WindowWidth = 55;

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        string[] shipRanks = { "Scout", "Submarine", "Destroyer", "BattleShip", "Aircraft Carrier" };
        char[] shipChar = { 'S', 'U', 'D', 'B', 'C' };
        
        List<Battleship> playerShips = new List<Battleship>();
        List<Battleship> aiShips = new List<Battleship>();
        List<Battleship> destroyedEnemyShips = new List<Battleship>();

        Player player = new Player("Playerrrrrrrrrr");
        Player ai = new Player("Easy Bot");

       // StartScreen();
        
        // Printing player/ai names
        //Console.SetCursorPosition(26 / 2-player.name.Length/2, 0);
        //Console.WriteLine(player.Name);
        
        //Console.SetCursorPosition(40, 0);
        //Console.WriteLine(ai.Name);

        PrintMatrix(player.Board, player.name);
        PrintAIMatrix(player.Board, ai.name);

        // Create a separate list for The AI
        for (int i = 0; i < 5; i++) 
        {
            aiShips.Add(MakeShipAI(shipRanks[i], shipSizes[i], ai, shipChar[i]));
            AddShipOnBoard(aiShips[i], ai);
        }

         //Read player ships
        for (int i = 0; i < 1; i++)
        {
            Console.SetCursorPosition(1, 14);
            playerShips.Add(PlaceShip(shipRanks[i], shipSizes[i], player, shipChar[i]));
            AddShipOnBoard(playerShips[i], player);
            Console.Clear();
            PrintMatrix(player.Board, player.name);
            PrintAIMatrix(ai.Board, ai.name);
        }

        // Test Shooting
        
        string shoot = string.Empty;
        bool end = true;
        while (end)
        {
            
            
            Console.SetCursorPosition(0, 14);
            
            Console.WriteLine("Enter target cooridnates, admiral!");
            Console.Write("Target coordinates: ");
            shoot = Console.ReadLine();
            
            if (CollisionCheck(aiShips,destroyedEnemyShips, ai, shoot))
            {
                Console.WriteLine("Direct hit!");
                Thread.Sleep(1000);
               
                
            }
            else
            {
                Console.WriteLine("We didn't get them this time!");
                Thread.Sleep(1000);
            }
            Console.Clear();
            PrintMatrix(player.Board, player.name);
            PrintAIMatrix(ai.Board, ai.name);
            end = true;
            
            
            
        }
    }

    static Battleship PlaceShip(string rank, int size, Player player, char sign)
    {
        Console.WriteLine("Battleship Rank: {0}, Size: {1}", rank, size);
        Console.Write("Input row and col coordinates: ");
        string[] coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        int coordinatesX = ConvertCoordinateToInt(coordinates[0]);
        int coordinatesY = int.Parse(coordinates[1].ToString()) - 1;
        Console.Write("Input Direction: ");
        char direction = Convert.ToChar(Console.ReadLine());

        bool validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, player);
        while (!validPosition)
        {
            Console.WriteLine("Invalid input.. Try again");
            Console.Write("Input row and col coordinates: ");
            coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            coordinatesX = ConvertCoordinateToInt(coordinates[0]);
            coordinatesY = int.Parse(coordinates[1]) - 1;
            Console.Write("Input Direction: ");
            direction = Convert.ToChar(Console.ReadLine());
            validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, player);
        }
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction, sign);
        return ship;
    }
    static Battleship MakeShipAI(string rank, int size, Player player, char sign)
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
        bool validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, player);
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
            validPosition = ValidatePosition(coordinatesX, coordinatesY, size, direction, player);
        }
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction, sign);
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
    static bool ValidatePosition(int shipX, int shipY, int shipLength, char shipDirection, Player player)
    {
        bool shipPlaced = true;

        for (int i = 0; i < shipLength; i++)
        {
            if (shipX < 0 || shipY < 0 || shipX >= player.board.GetLength(0) || shipY >= player.board.GetLength(1) || player.board[shipX, shipY] != 'O')
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
    static void AddShipOnBoard(Battleship ship, Player player)
    {
        for (int i = 0; i < ship.Size; i++)
        {
            player.board[ship.CurrentX, ship.CurrentY] = ship.signature;
            switch (ship.Direction)
            {
                case 'R': ship.y ++; break;
                case 'D': ship.x ++; break;
                case 'L': ship.y --; break;
                case 'U': ship.x --; break;
                default: break;
            }
        }
    }
    static bool CheckForCollision(List<Battleship> shipList, Player attackedPlayer, string coordinates) 
    {
        // Check if the attacked player has a ship that will be hit
        bool collision = false;

        for (int i = 0; i < shipList.Count; i++) // Checking all ships from the list one by one
        {
            int shipX = shipList[i].CurrentX;
            int shipY = shipList[i].CurrentY;
            string name = shipList[i].rank;
            int fireX = ConvertCoordinateToInt(coordinates[0].ToString());
                int fireY = int.Parse(coordinates[2].ToString()) - 1;

            for (int square = 0; square < shipList[i].Size; square++) // Check every square the ship occupies one by one 
            {
                if (fireX == shipX && fireY == shipY) // Check if the input coordinates are on target   
                {
                    collision = true;
                }
                if (square != shipList[i].Size - 1) // If there are still squares left from the ship
                {
                    switch (shipList[i].Direction) // Get coordinates of the next square
                    {
                        case 'R': shipY++; break;
                        case 'D': shipX++; break;
                        case 'L': shipY--; break;
                        case 'U': shipX--; break;
                        default: break;
                    }
                }
            }
        }
        return collision;
    }
    static bool CollisionCheck(List<Battleship> shiplist, List<Battleship> destroyedShips, Player attackedPlayer, string shotCoordinates)
    {
        bool collision = false;
        string[] coordinates = shotCoordinates.Split();
        int row = ConvertCoordinateToInt(coordinates[0].ToString());
        int col = int.Parse(coordinates[1])-1;
        
        for (int i = 0; i < shiplist.Count; i++)
        {
            if (attackedPlayer.board[row,col] == shiplist[i].signature)
            {
                collision =true;
                shiplist[i].health--;
                //some sort of logic to track destroyed ships
                //will work it out
                //later on

                //if (shiplist[i].health == 0)
                //{
                //    destroyedShips.Add(shiplist[i]);
                //}
                attackedPlayer.board[row, col] = 'X';
                break;
            }
        }
        
        
        return collision;
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
            case "A": row = 0; break;
            case "B": row = 1; break;
            case "C": row = 2; break; 
            case "D": row = 3; break;
            case "E": row = 4; break;
            case "F": row = 5; break;
            case "G": row = 6; break;
            case "H": row = 7; break;
            case "I": row = 8; break;
            case "J": row = 9; break;
            default: break;
        }
        return row;
    }
}