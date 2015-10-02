using System;
using System.Collections.Generic;
using GameClasses;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

class TheGame
{
    static void Main()
    {
        Console.BufferHeight = Console.WindowHeight = 35;
        Console.BufferWidth = Console.WindowWidth = 55;

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        string[] shipRanks = { "Scout", "Submarine", "Destroyer", "BattleShip", "Aircraft Carrier" };
        char[] shipChar = { 'S', 'U', 'D', 'B', 'C' };


        //StartScreen();

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
            Console.WriteLine("Battleship Rank: {0}, Size: {1}", shipRanks[i], shipSizes[i]);

            string input = GetValidInput(); // Get coordinates in correct format
            while (true) 
            {
                if (ValidPosition(input, shipSizes[i], input[2], player)) // Check if coordinates are in the boundries
                {
                    break;
                }
                Console.SetCursorPosition(1, 15);
                Console.WriteLine("Outside of boundries! .. Give it another go ..");
                input = GetValidInput();
            }
            playerShips.Add(CreateShip(input, shipRanks[i], shipSizes[i], player, shipChar[i]));
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


            int rowShoot = ConvertToInt(shoot[0].ToString());
            int colShoot = int.Parse(shoot[1].ToString());

            while (ai.Board[rowShoot, colShoot] == '$' || ai.Board[rowShoot, colShoot] == 'X')
            {
                Console.WriteLine("Invalid input! Try again. ");
                Console.Write("Target coordinates: ");
                shooter = Console.ReadLine();
                shoot = shooter.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                rowShoot = ConvertToInt(shoot[0].ToString());
                colShoot = int.Parse(shoot[1].ToString());
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

    static Battleship CreateShip(string input, string rank, int size, Player player, char sign)
    {
        // TO DELETE IF IT WORKS
        //Console.WriteLine("Battleship Rank: {0}, Size: {1}", rank, size);
        //Console.Write("Input row and col coordinates: ");
        // TO DELETE IF IT WORKS
        //string[] coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        int coordinatesX = ConvertToInt(input[0].ToString());
        int coordinatesY = int.Parse(input[1].ToString());

        // TO DELETE IF IT WORKS
        //Console.Write("Input Direction: ");
        //char direction = Convert.ToChar(Console.ReadLine());
        //bool validPosition = ValidPosition(coordinatesX, coordinatesY, size, input[2], player);
        //while (!validPosition)
        //{
        //    Console.WriteLine("Invalid input! Try again");
        //    Console.Write("Input row and col coordinates: ");
        //    coordinates = Console.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        //    coordinatesX = ConvertCoordinateToInt(coordinates[0].ToString());
        //    coordinatesY = int.Parse(coordinates[1].ToString()) - 1;
        //    Console.Write("Input Direction: ");
        //    direction = Convert.ToChar(Console.ReadLine());
        //    validPosition = ValidPosition(coordinatesX, coordinatesY, size, direction, player);
        //}

        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, input[2], sign);
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

        // DELETE IF IT WORKS
        //bool validPosition = ValidPosition(coordinatesX, coordinatesY, size, direction, player);
        //while (!validPosition)
        //{
        //    coordinatesX = rnd.Next(0, 10);
        //    coordinatesY = rnd.Next(0, 10);
        //    intDirection = rnd.Next(1, 5);
        //    switch (intDirection)
        //    {
        //        case 1: direction = 'R'; break;
        //        case 2: direction = 'D'; break;
        //        case 3: direction = 'L'; break;
        //        case 4: direction = 'U'; break;
        //        default: break;
        //    }
        //    validPosition = ValidPosition(coordinatesX, coordinatesY, size, direction, player);
        //}
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction, sign);
        return ship;
    }
    static void PrintMatrix(char[,] matrix, string playerName)
    {
        //name printing
        Console.SetCursorPosition(26 / 2 - playerName.Length / 2, 0);
        Console.WriteLine(playerName);
        //name printing

        Console.WriteLine("    0 1 2 3 4 5 6 7 8 9");
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
    static bool ValidPosition(string input, int shipLength, char shipDirection, Player player)
    {
        int shipX = ConvertToInt(input[0].ToString());
        int shipY = int.Parse(input[1].ToString());
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
                case 'r': shipY++; break;
                case 'd': shipX++; break;
                case 'l': shipY--; break;
                case 'u': shipX--; break;
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
                case 'r': ship.y++; break;
                case 'd': ship.x++; break;
                case 'l': ship.y--; break;
                case 'u': ship.x--; break;
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
        Console.WriteLine("    0 1 2 3 4 5 6 7 8 9");
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
        char[] charToReturn = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'J', 'H', 'I', 'J', };
        return charToReturn[i];
    }

    static int ConvertToInt(string input)
    {
        int row = 0;
        switch (input)
        {
            case "a": row = 0; break;
            case "b": row = 1; break;
            case "c": row = 2; break;
            case "d": row = 3; break;
            case "e": row = 4; break;
            case "f": row = 5; break;
            case "g": row = 6; break;
            case "h": row = 7; break;
            case "i": row = 8; break;
            case "j": row = 9; break;
            default: break;
        }
        return row;
    }

    static string GetValidInput()
    {
        // Method works with lowercase and uppercase characters from a-j, 
        // including whitespaces in the beginning, middle or end
        Regex withDirectionRGX = new Regex(@"^[a-jA-J]\s*[\d][0]?\s*[udlrUDLR]\s*$");
        Regex withoutDirectionRGX = new Regex(@"^[a-jA-J]\s*[\d][0]?\s*$");
        Regex directionRGX = new Regex(@"^\s*[udlrUDLR]\s*$");

        Console.SetCursorPosition(1, 15);
        Console.Write("Where to place your ship?");
        while (true)
        {
            string command = Console.ReadLine();
            if (withDirectionRGX.Match(command).Success)
            {
                command = command.Replace(@"s+", "").ToLower();
                return command;
            }
            else if (withoutDirectionRGX.Match(command).Success)
            {
                Console.SetCursorPosition(1, 16);
                Console.Write("Give me direction!");
                while (true)
                {
                    string direction = Console.ReadLine();
                    if (directionRGX.Match(direction).Success)
                    {
                        command = command.Replace(@"s+", "").ToLower();
                        direction = direction.Replace(@"s+", "").ToLower();
                        return command + direction;
                    }
                    Console.SetCursorPosition(1, 16);
                    Console.Write("Ughh, can you repeat directions!");
                }
            }
            Console.SetCursorPosition(1, 15);
            Console.Write("Arrgh! I didnt get that..");
        }
    }
}