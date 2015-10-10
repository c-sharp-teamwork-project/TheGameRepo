using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClasses
{
    class Player
    {
        public string name;
        public char[,] board = {
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                                   {'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O' },
                               };

        public int shipsLeft;
        public List<Battleship> ships = new List<Battleship>();

        public Player(string name)
        {
            this.name = name;
        }

        public List<Battleship> Ships
        {
            get { return ships; }
            set { ships = value; }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public int ShipsLeft
        {
            get
            {
                return shipsLeft;
            }
            set
            {
                shipsLeft = value;
            }
        }
        public char[,] Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
            }
        }
    }
}
