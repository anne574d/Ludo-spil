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
        public Field(int i)
        {
            pieces = new List<Piece>();
            currentColor = "";
            index = i;
        }

        public void IncomingPiece(Piece piece)
        {
            if (EnemyDominated(piece.Color))
            {
                piece.SendToStart();
            }
            else if (SingleEnemy(piece.Color))
            {
                pieces[0].SendToStart();
                pieces.RemoveAt(0);

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

        public bool EnemyDominated(string incomingColor)
        {
            return (pieces.Count > 1 && currentColor != incomingColor);
        }

        public bool SingleEnemy(string incomingColor)
        {
            return (pieces.Count == 1 && currentColor != incomingColor);
        }

        public bool IsHomeField()
        {
            return (index == 57 || index == 63 || index == 69 || index == 75);
        }

        public bool IsHomeLane()
        {
            return (index > 51);
        }

        public bool HasFriendlyPiece(string incomingColor)
        {
            return (incomingColor == currentColor && pieces.Count > 0);
        }

        List<Piece> pieces;
        string currentColor;
        int index;
    }
}
