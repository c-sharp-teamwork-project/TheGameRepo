using System;
using System.Collections.Generic;
using GameClasses;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

class TheGame
{
    static List<string> log = new List<string>();

    static void Main()
    {
        Console.BufferHeight = Console.WindowHeight = 23;
        Console.BufferWidth = Console.WindowWidth = 93;

        int[] shipSizes = { 2, 3, 3, 4, 5 };
        string[] shipRanks = { "Scout", "U-Boat", "Destroyer", "BattleShip", "Aircraft Carrier" };
        char[] shipChar = { 'S', 'U', 'D', 'B', 'C' };
        
        string logEvent;

        StartScreen();

        List<Battleship> destroyedEnemyShips = new List<Battleship>();
        List<Battleship> destroyedPlayerShips = new List<Battleship>();

        List<string> destroyedShips = new List<string>();
        const ConsoleColor ENEMY = ConsoleColor.Red;

        Console.SetCursorPosition(10, 20 / 2);
        Console.Write("Please enter your name: ");
        string playerName = Console.ReadLine();

        Player player = new Player(playerName);
        Player emptyPlayer = new Player("CPU");

        PrintScreen(player, emptyPlayer);

        // ** Read AI ships **
        for (int i = 0; i < 5; i++)
        {
            emptyPlayer.ships.Add(MakeShipAI(shipRanks[i], shipSizes[i], emptyPlayer, shipChar[i]));
            AddShipOnBoard(emptyPlayer.ships[i], emptyPlayer);
        }
        // ** End of reading AI ships **

        //** Read player ships **
        for (int i = 0; i < 1; i++)
        {
            Console.SetCursorPosition(15, 19);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(15, 19);
            Console.Write("Place Battleship Rank <{0}> Size <{1}>", shipRanks[i], shipSizes[i]);

            string input = GetValidInput(); // Get coordinates in correct format
            while (true) 
            {
                if (ValidatePosition(input, shipSizes[i], input[2], player)) // Check if coordinates are in the boundries
                {
                    break;
                }

                logEvent = "Ship cannot be placed there!";
                RefreshLog(log, logEvent);

                Thread.Sleep(1500);
                input = GetValidInput();
            }
            player.ships.Add(CreateShip(input, shipRanks[i], shipSizes[i], player, shipChar[i]));
            AddShipOnBoard(player.ships[i], player);
            PrintScreen(player, emptyPlayer);

            logEvent = "<" + player.Name + ">" + " placed " + "<" + shipRanks[i] + "> at " + "<" + input.ToUpper() + ">";
            RefreshLog(log, logEvent);
        }
        //** End of reading player ships **
        
        Console.ResetColor();
        string outcome = "";

        while (true)
        {
            //** Player shooting **
            PrintLog(log);
            string shootCoordsPlayer = ShootInputForPlayer(emptyPlayer);

            if (CollisionCheckForPlayer(emptyPlayer.ships, emptyPlayer, shootCoordsPlayer, emptyPlayer, destroyedEnemyShips))
            {
                Console.Beep();
                outcome = "<" + player.Name + "> " + "Direct hit!" + " at " + "<" + shootCoordsPlayer.ToUpper() + "> ";
                RefreshLog(log, outcome);

                Thread.Sleep(1000);
                if (destroyedEnemyShips.Count == emptyPlayer.ships.Count)
                {
                    PrintScreen(player, emptyPlayer);
                    string winner = "WE'VE SUNKEN THE ENTIRE ENEMY FLEET!";
                    string winner2 = "THE DAY IS OURS!";
                    Console.WriteLine();
                    Console.WriteLine(new string(' ', (55 / 2) - (winner.Length / 2)) + winner);
                    Console.WriteLine(new string(' ', (55/2) - (winner2.Length/2))+ winner2);
                    break;
                }
            }
            else
            {
                outcome = "<" + player.Name + "> " + "Missed!" + " at " + "<" + shootCoordsPlayer.ToUpper() + "> ";
                RefreshLog(log, outcome);
                Thread.Sleep(1000);
            } 
            //** End of player shooting **

            //** AI shooting **
            Random rnd = new Random();
            int rowShoot = rnd.Next(0, 10);
            int colShoot = rnd.Next(0, 10);
            while (player.Board[rowShoot, colShoot] == '$' || player.board[rowShoot, colShoot] == 'X')
            {
                rnd = new Random();
                rowShoot = rnd.Next(0, 10);
                colShoot = rnd.Next(0, 10);
            }

            if (CollisionCheckForAI(player.ships, player, rowShoot, colShoot, destroyedPlayerShips))
            {
                Console.Beep(); ;
                outcome = "<" + emptyPlayer.Name + "> " + "Direct hit!" + " at " + "<" + GetRowChar(rowShoot) + (colShoot) + ">";
                RefreshLog(log, outcome);
                if (destroyedPlayerShips.Count == player.ships.Count)
                {
                    PrintScreen(player, emptyPlayer);
                    string loser = "YOU LOSE";
                    Console.WriteLine(new string(' ', (55 / 2) - (loser.Length / 2)) + loser);
                    break;
                }
                Thread.Sleep(1000);

            }
            else
            {
                outcome = "<" + emptyPlayer.Name + "> " + "Missed!" + " at " + "<" + GetRowChar(rowShoot) + (colShoot) + ">";
                RefreshLog(log, outcome);
                Thread.Sleep(1000);
            }
            //** End of AI shooting **

            //** Printing the console
            PrintScreen(player, emptyPlayer);
        }
    }

    private static void PrintLog(List<string> log)
    {
        for (int i = 0; i < log.Count; i++)
        {
            Console.SetCursorPosition(15, 17 - i);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(15, 17 - i);
            Console.WriteLine(log[i]);
        }
    }

    private static void RefreshLog(List<string> log, string newEvent)
    {
        if (log.Count >= 4)
        {
            log.RemoveAt(3);
        }
        log.Insert(0, newEvent);

        for (int i = 0; i < log.Count; i++)
        {
            Console.SetCursorPosition(15, 17 - i);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(15, 17 - i);
            Console.WriteLine(log[i]);
        }
    }


    static void DestroyedShipsByPlayer(List<Battleship> destroyedShips, string name)
    {
        Console.SetCursorPosition(0, 20);
        Console.WriteLine("Ships sunken by {0}", name);
        foreach (var destroyedShip in destroyedShips)
        {
            Console.WriteLine("-{0}",destroyedShip.rank);
        }
    }


    static void DestroyedShipsByAI(List<Battleship> AiDestroyedShips, string aiName)
    {
        Console.SetCursorPosition(35, 20);
        Console.WriteLine("Ships sunken by {0}", aiName);
        foreach (var destroyedShip in AiDestroyedShips)
        {
            Console.WriteLine("-{0}",destroyedShip.rank);
        }
    }


    private static void PrintScreen(Player player, Player emptyPlayer)
    {
        Console.Clear();
        PrintMatrix(player.Board, player.name);
        Console.ForegroundColor = ConsoleColor.Red;
        PrintAIMatrix(emptyPlayer.Board, emptyPlayer.name);
        Console.ResetColor();
    }


    static Battleship CreateShip(string input, string rank, int size, Player player, char sign)
    {
        // Create an object of class Battleship for the given player
        int coordinatesX = ConvertToInt(input[0].ToString());
        int coordinatesY = int.Parse(input[1].ToString());
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, input[2], sign);
        return ship;
    }


    static Battleship MakeShipAI(string rank, int size, Player player, char sign)
    {
        // Create an object of class Battleship for the AI
        Random rnd = new Random();
        int coordinatesX = rnd.Next(0,10);
        int coordinatesY = rnd.Next(0, 10);
        int intDirection = rnd.Next(1, 5);
        char direction = ' ';
        switch (intDirection)
        {
            case 1: direction = 'r'; break;
            case 2: direction = 'd'; break;
            case 3: direction = 'l'; break;
            case 4: direction = 'u'; break;
            default: break;
        }

        string aiCoordinates = coordinatesX.ToString() + coordinatesY.ToString();
        bool validPosition = ValidatePositionAI(aiCoordinates, size, direction, player);
        while (validPosition == false)
        {
            coordinatesX = rnd.Next(0,10);
            coordinatesY = rnd.Next(0, 10);
            intDirection = rnd.Next(1, 5);
            direction = ' ';
            switch (intDirection)
            {
                case 1: direction = 'r'; break;
                case 2: direction = 'd'; break;
                case 3: direction = 'l'; break;
                case 4: direction = 'u'; break;
                default: break;
            }
            aiCoordinates = coordinatesX.ToString() +coordinatesY.ToString();
            validPosition = ValidatePositionAI(aiCoordinates, size, direction, player);
        }
        Battleship ship = new Battleship(rank, coordinatesX, coordinatesY, size, direction, sign);
        return ship;
    }

    
    static bool ValidatePositionAI(string input, int shipLength, char shipDirection, Player player)
    {
        // Validation of the random generated coordinates for the AI
        int shipX = int.Parse(input[0].ToString());
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
            if (shipX < 0 || shipY < 0 || shipX >= player.board.GetLength(0) || shipY >= player.board.GetLength(1) || player.board[shipX, shipY] != 'O')
            {
                shipPlaced = false;
                break;
            }
        }
        return shipPlaced;
    }


    static bool ValidatePosition(string input, int shipLength, char shipDirection, Player player)
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
            if (shipX < 0 || shipY < 0 || shipX >= player.board.GetLength(0) || shipY >= player.board.GetLength(1) || player.board[shipX, shipY] != 'O')
            {
                shipPlaced = false;
                break;
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


    static bool CollisionCheckForPlayer(List<Battleship> shiplist, Player ai, string coordinates, Player emptyPlayer, List<Battleship> destroyedShips)
    {
        int row = ConvertToInt(coordinates[0].ToString());
        int col = int.Parse(coordinates[1].ToString());

        bool collision = false;

        for (int i = 0; i < shiplist.Count; i++)
        {
            if (ai.board[row, col] == shiplist[i].signature)
            {
                collision = true;
                ai.board[row, col] = 'X'; //Put X sign if hit on the ai table
                emptyPlayer.board[row, col] = 'X'; //Put X sign if hit on the empty table
                shiplist[i].health--;
                if (shiplist[i].health == 0)
                {
                    destroyedShips.Add(shiplist[i]);
                }
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


    static bool CollisionCheckForAI(List<Battleship> shiplist, Player player, int row, int col, List<Battleship> destroyedShips)
    {
        bool collision = false;

        for (int i = 0; i < shiplist.Count; i++)
        {
            if (player.board[row, col] == shiplist[i].signature)
            {
                collision = true;
                player.board[row, col] = 'X';
                player.board[row, col] = 'X'; //Put X sign if hit

                shiplist[i].health--;
                if (shiplist[i].health == 0)
                {
                    destroyedShips.Add(shiplist[i]);
                }
                break;
            }
            if (i == shiplist.Count - 1 && player.board[row, col] != shiplist[i].signature)
            {
                player.board[row, col] = '$'; //Put $ sign if miss
            }
        }
        return collision;
    }


    static void StartScreen()
    {
        string gameName = "L'Attaque";
        Console.SetCursorPosition((93 / 2) - (gameName.Length / 2), 2);

        Console.WriteLine("L'Attaque");

        StreamReader reader = new StreamReader("../../Instuctions.txt");

        using (reader)
        {
            int counter = 2;
            string line = reader.ReadLine();
            while (line != null)
            {
                Console.SetCursorPosition((93 / 2) - (line.Length / 2), counter);
                Console.WriteLine(line);
                line = reader.ReadLine();
                counter++;
            }
        }
        Console.SetCursorPosition((93 / 2) - ("Press Enter".Length / 2), 19);
        Console.Write("Press Enter");
        Console.ReadLine();
        Console.Clear();

    }


    static void PrintMatrix(char[,] matrix, string playerName)
    {
        // Print player name
        int offset = 12;
        Console.SetCursorPosition(offset + (32 / 2 - playerName.Length / 2), 1);
        Console.WriteLine(playerName);
        

        // Print matrix
        Console.SetCursorPosition(offset, 2);
        Console.WriteLine("        0 1 2 3 4 5 6 7 8 9");
        //Console.SetCursorPosition(offset, 3);
        //Console.WriteLine("    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            Console.SetCursorPosition(offset, i + 3);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("///  {0} ", GetRowChar(i));
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
            Console.WriteLine("  ///");
        }
    }


    static void PrintAIMatrix(char[,] matrix, string aiName)
    {
        //Print player name
        int offset = 50;
        Console.SetCursorPosition(offset + (32 / 2 - aiName.Length / 2), 1);
        Console.WriteLine(aiName);

        Console.SetCursorPosition(offset, 2);
        Console.WriteLine("        0 1 2 3 4 5 6 7 8 9");
        //Console.SetCursorPosition(offset, 3);
        //Console.WriteLine("///    " + new string('-', (matrix.GetLength(0) * 2) - 1));
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            Console.SetCursorPosition(offset, i + 3);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == 0)
                {
                    Console.Write("///  {0} ", GetRowChar(i));
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
            Console.WriteLine("  ///");
        }
    }


    static char GetRowChar(int i)
    {
        char[] charToReturn = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', };
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


    static string ShootInputForPlayer(Player ai)
    {
        // Method works with lowercase and uppercase characters from a-j, 
        // including whitespaces in the beginning, middle or end
        Regex shootRGX = new Regex(@"^\s*[a-jA-J]\s*[\d]\s*$");
        Regex whitespace = new Regex(@"\s*");

        // Start of player shooting again
        PrintPrePromt(2);
        PrintPrompt(3);
        string command = Console.ReadLine();

        while (true)
        {
            if (shootRGX.Match(command).Success)
            {
                //command = command.Replace(@"\s*", "").ToLower();
                command = whitespace.Replace(command, "").ToLower();

                int rowShoot = ConvertToInt(command[0].ToString());
                int colShoot = int.Parse(command[1].ToString());

                while (ai.Board[rowShoot, colShoot] == '$' || ai.Board[rowShoot, colShoot] == 'X')
                {

                    PrintPrePromt(1);
                    PrintPrompt(3);
                    command = Console.ReadLine();
                    command = whitespace.Replace(command, "").ToLower();

                    if (shootRGX.Match(command).Success)
                    {
                        rowShoot = ConvertToInt(command[0].ToString());
                        colShoot = int.Parse(command[1].ToString());
                    }
                }
                break;
            }
            PrintPrePromt(0);
            PrintPrompt(3);
            command = Console.ReadLine();
        }
        return command;
    }

    
    static string GetValidInput()
    {
        // Method works with lowercase and uppercase characters from a-j, 
        // including whitespaces in the beginning, middle or end
        Regex withDirectionRGX = new Regex(@"^\s*[a-jA-J]\s*[\d]\s*[udlrUDLR]\s*$");
        Regex whitespace = new Regex(@"\s*");

        PrintPrompt(0);
        while (true)
        {
            string command = Console.ReadLine();
            if (withDirectionRGX.Match(command).Success)
            {
                command = command.Replace(@"\s*", "").ToLower();

                command = whitespace.Replace(command, "");

                return command;
            }
            PrintPrompt(1);
        }
    }


    private static void PrintPrePromt(int prompt)
    {
        Console.SetCursorPosition(15, 19);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(15, 19);

        string[] prompts = new string[]
        {
            "Put the rum away and focus, admiral!",
            "Invalid input! Try again. ",
            "Enter target coordinates, admiral!",
        };

        Console.Write(prompts[prompt]);
    }


    private static void PrintPrompt(int prompt)
    {
        Console.SetCursorPosition(15, 20);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(15, 20);

        string[] prompts = new string[]
        {
            "Place your ship!(A-J,0-9,R/D/U/L): ",
            "Arrgh! I didnt get that..: ",
            "Put the rum away and focus, admiral!",
            "Target coordinates: ",
        };

        Console.Write(prompts[prompt]);
    }
}