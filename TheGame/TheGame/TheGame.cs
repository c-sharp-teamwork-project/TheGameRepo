using System;
using System.Collections.Generic;
using GameClasses;
using System.Threading;
using System.IO;

class TheGame
{
    static void Main()
    {
        Console.BufferHeight = Console.WindowHeight = 35;
        Console.BufferWidth = Console.WindowWidth = 55;

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        string[] shipRanks = { "Scout", "Submarine", "Destroyer", "BattleShip", "Aircraft Carrier" };
        char[] shipChar = { 'S', 'U', 'D', 'B', 'C' };


        StartScreen();

        List<Battleship> playerShips = new List<Battleship>();
        List<Battleship> aiShips = new List<Battleship>();

        Console.Write("Please enter your name: ");
        string playerName = Console.ReadLine();

        Console.Clear();

        Player player = new Player(playerName);
        Player ai = new Player("Easy Bot");
        Player emptyPlayer = new Player("Easy Bot");

        PrintMatrix(player.Board, player.name);
        PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);

        // Create a separate list for The AI
        for (int i = 0; i < 5; i++)
        {
            aiShips.Add(MakeShipAI(shipRanks[i], shipSizes[i], ai, shipChar[i]));
            AddShipOnBoard(aiShips[i], ai);
        }

        //Read player ships
        for (int i = 0; i < 5; i++)
        {
            Console.SetCursorPosition(1, 14);
            playerShips.Add(PlaceShip(shipRanks[i], shipSizes[i], player, shipChar[i]));
            AddShipOnBoard(playerShips[i], player);
            Console.Clear();
            PrintMatrix(player.Board, player.name);
            PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);
        }

        // Test Shooting

        string shooter = string.Empty;
        bool end = true;

        List<string> destroyedShips = new List<string>();
        List<Battleship> aiShipsToRemove = new List<Battleship>();
        foreach (var ship in aiShips)
        {
            aiShipsToRemove.Add(ship);
        }
        List<Battleship> playerShipsToRemove = new List<Battleship>();
        foreach (var ship in playerShips)
        {
            playerShipsToRemove.Add(ship);
        }

        string playerLastTurnOutcome = "";
        string aiLastTurnOutcome = "";

        while (end)
        {

            Console.SetCursorPosition(0, 13);
            Console.WriteLine(playerLastTurnOutcome);
            Console.SetCursorPosition(30, 13);
            Console.WriteLine(aiLastTurnOutcome);
            Console.SetCursorPosition(0, 14);
            Console.WriteLine("Destroyed ships: ");
            Console.WriteLine(string.Join(", ", destroyedShips)); // printing the destroyed ships

            //string winner = "YOU WIN";
            if (aiShipsToRemove.Count == 0)
            {
                Console.Clear();
                PrintMatrix(player.Board, player.name);
                PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);
                string winner = "YOU WIN";
                Console.WriteLine(new string(' ', (55 / 2) - (winner.Length / 2)) + winner);
                break;
            }
            if (playerShipsToRemove.Count == 0)
            {
                Console.Clear();
                PrintMatrix(player.Board, player.name);
                PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);
                string loser = "YOU LOSE";
                Console.WriteLine(new string(' ', (55 / 2) - (loser.Length / 2)) + loser);
                break;
            }

            Console.WriteLine("Enter target coordinates, admiral!");
            Console.Write("Target coordinates: ");
            shooter = Console.ReadLine();
            string[] shoot = shooter.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);


            int rowShoot = ConvertCoordinateToInt(shoot[0].ToString());
            int colShoot = int.Parse(shoot[1].ToString()) - 1;

            while (ai.Board[rowShoot, colShoot] == '$' || ai.Board[rowShoot, colShoot] == 'X')
            {
                Console.WriteLine("Invalid input! Try again. ");
                Console.Write("Target coordinates: ");
                shooter = Console.ReadLine();
                shoot = shooter.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                rowShoot = ConvertCoordinateToInt(shoot[0].ToString());
                colShoot = int.Parse(shoot[1].ToString()) - 1;
            }

            if (CollisionCheckForPlayer(aiShips, ai, rowShoot, colShoot, emptyPlayer))
            {
                Console.Beep();
                playerLastTurnOutcome = "Direct hit! " + "- " + GetRowChar(rowShoot) + (colShoot + 1);
                foreach (var ship in aiShips) //Adding the destroyed ships to List
                {
                    if (ship.health == 0)
                    {
                        if (!destroyedShips.Contains(ship.rank))
                        {
                            destroyedShips.Add(ship.rank);
                            aiShipsToRemove.Remove(ship);
                        }
                    }
                }
                Thread.Sleep(1000);

            }
            else
            {
                playerLastTurnOutcome = "We didn't get them this time! " + "- " + GetRowChar(rowShoot) + (colShoot + 1);
                Thread.Sleep(1000);
            }

            Console.Clear();
            PrintMatrix(player.Board, player.name);

            PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);

            Random rnd = new Random(); //getting the ai row and col to shoot
            rowShoot = rnd.Next(0, 10);
            colShoot = rnd.Next(0, 10);
            while (player.Board[rowShoot, colShoot] == '$' || player.board[rowShoot, colShoot] == 'X')
            {
                rnd = new Random();
                rowShoot = rnd.Next(0, 10);
                colShoot = rnd.Next(0, 10);
            }

            if (CollisionCheckForAI(playerShips, player, rowShoot, colShoot))
            {
                Console.Beep(); ;
                aiLastTurnOutcome = "Direct hit! " + "- " + GetRowChar(rowShoot) + (colShoot + 1);

                Thread.Sleep(1000);

            }
            else
            {
                aiLastTurnOutcome = "The AI missed! " + "- " + GetRowChar(rowShoot) + (colShoot + 1);
                Thread.Sleep(1000);
            }

            Console.Clear();
            PrintMatrix(player.Board, player.name);

            PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);

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
            Console.WriteLine("Invalid input! Try again");
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
                case 'R': ship.y++; break;
                case 'D': ship.x++; break;
                case 'L': ship.y--; break;
                case 'U': ship.x--; break;
                default: break;
            }
        }
    }

    static bool CollisionCheckForPlayer(List<Battleship> shiplist, Player ai, int row, int col, Player emptyPlayer)
    {
        bool collision = false;

        for (int i = 0; i < shiplist.Count; i++)
        {
            if (ai.board[row, col] == shiplist[i].signature)
            {
                collision = true;
                shiplist[i].health--;
                ai.board[row, col] = 'X'; //Put X sign if hit on the ai table
                emptyPlayer.board[row, col] = 'X'; //Put X sign if hit on the empty table
                break;
            }
            if (i == shiplist.Count - 1 && ai.board[row, col] != shiplist[i].signature)
            {
                ai.board[row, col] = '$'; //Put $ sign of miss on the ai table
                emptyPlayer.board[row, col] = '$'; //Put $ sign of miss on the empty table
            }

        }

        return collision;
    }
    static bool CollisionCheckForAI(List<Battleship> shiplist, Player player, int row, int col)
    {
        bool collision = false;

        for (int i = 0; i < shiplist.Count; i++)
        {
            if (player.board[row, col] == shiplist[i].signature)
            {
                collision = true;
                shiplist[i].health--;
                player.board[row, col] = 'X'; //Put X sign if hit
                break;
            }
            if (i == shiplist.Count - 1 && player.board[row, col] != shiplist[i].signature)
            {
                player.board[row, col] = '$'; //Put $ sign of miss
            }

        }

        return collision;
    }


    static void StartScreen()
    {
        string gameName = "The Amazing Battleships";
        Console.SetCursorPosition((55 / 2) - (gameName.Length / 2), 0);

        Console.WriteLine("The Amazing Battleships");

        StreamReader reader = new StreamReader("../../Instuctions.txt");

        using (reader)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                Console.WriteLine(line);
                line = reader.ReadLine();
            }
        }
        Console.ReadLine();
        Console.Clear();

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
            Console.SetCursorPosition(30, i + 3);
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