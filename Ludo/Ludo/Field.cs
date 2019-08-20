using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ludo
{
    class Field
    {
        public Field()
        {
            pieces = new List<Piece>();
            currentColor = "";
        }

        public void IncomingPiece(Piece piece)
        {
            if (enemyDominated(piece.Color))
            {
                piece.SendToStart();
            }
            else if (singleEnemy(piece.Color))
            {
                pieces[0].SendToStart();
                pieces.Remove(pieces[0]);

                currentColor = piece.Color;
                pieces.Add(piece);
            }
            else
            {
                // lands on empty field or field with own pieces
                currentColor = piece.Color;
                pieces.Add(piece);
            }
        }
        public void OutgoingPiece(Piece piece)
        {
            pieces.Remove(piece);
        }

        private bool enemyDominated(string incomingColor)
        {
            return (pieces.Count > 1 && currentColor != incomingColor);
        }

        private bool singleEnemy(string incomingColor)
        {
            return (pieces.Count == 1 && currentColor != incomingColor);
        }

        List<Piece> pieces;
        string currentColor;

    }
}
