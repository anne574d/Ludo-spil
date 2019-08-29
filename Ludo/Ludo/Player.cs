using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Ludo
{
    public class Player
    {
        public Player(string color, bool humanPlayer, GUI gui)
        {
            Color = color;
            Human = humanPlayer;

            Pieces = new List<Piece>();
            for (int i = 1; i <= 4; ++i)
            {
                Pieces.Add(new Piece(color, i, gui));
            }
        }

        public Piece GetPiece(int number)
        {
            // on board the pieces range 1-4, but has indexes 0-3
            return Pieces[number - 1];
        }

        public Piece GetPieceAtStart()
        {
            foreach (var p in Pieces)
            {
                if (p.IsAtStart())
                {
                    return p;
                }
            }
            throw new Exception("No piece at start");
        }

        public bool IsDone()
        {
            bool res = true;
            foreach (var p in Pieces)
            {
                res &= p.IsHome;
            }
            return res;
        }

        public List<int> ValidMoves(int diceroll)
        {
            List<int> result = new List<int>();
            foreach (var piece in moveablePieces(diceroll))
            {
                result.Add(GetPiece(piece).Position);
            }
            return result;
        }

        private List<int> moveablePieces(int diceroll)
        {
            List<int> res = new List<int>();
            bool valid;

            foreach (var p1 in Pieces)
            {
                if (p1.IsHome)
                {
                    valid = false;
                }
                else if (p1.Position == -1)
                {
                    valid = p1.CanLeaveStart(diceroll);
                }
                else
                {
                    valid = true;
                    foreach (var p2 in Pieces)
                    {
                        valid &= !p1.Overtakes(p2, diceroll);
                    }
                }
                if (valid)
                {
                    res.Add(p1.Number);
                }
            }
            return res;
        }

        public int SortOrder()
        {
            // sort order of colors when moving clockwise around board
            int res = 4;
            switch (Color)
            {
                case "yellow": res = 0; break;
                case "blue": res = 1; break;
                case "red": res = 2; break;
                case "green": res = 3; break;
            }
            return res;
        }

        public List<Piece> Pieces;
        public string Color { get; private set; }
        public bool Human { get; private set; }
    }
}