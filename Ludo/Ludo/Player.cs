using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ludo
{
    class Player
    {
        public Player(string color)
        {
            Color = color;

            Piece p1 = new Piece(color, 1);
            Piece p2 = new Piece(color, 2);
            Piece p3 = new Piece(color, 3);
            Piece p4 = new Piece(color, 4);

            pieces = new List<Piece>();
            pieces.Add(p1);
            pieces.Add(p2);
            pieces.Add(p3);
            pieces.Add(p4);
        }

        public Piece GetPiece(int number)
        {
            // on board the pieces range 1-4, but has indexes 0-3
            return pieces[number - 1];
        }

        public bool IsDone()
        {
            bool res = true;
            foreach (var p in pieces)
            {
                res &= p.IsHome();
            }
            return res;
        }

        public List<int> MovablePieces(int diceroll)
        {
            List<int> res = new List<int>();
            bool valid;
            foreach(var p1 in pieces)
            {
                valid = true;
                foreach (var p2 in pieces)
                {
                    if (p1.CanExitStart(diceroll))
                    {
                        valid = true;
                        break;
                    }
                    else if (p1 != p2)
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

        public string Color { get; private set; }
        List<Piece> pieces;
    }
}
