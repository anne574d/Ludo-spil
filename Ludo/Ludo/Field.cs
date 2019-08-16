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
            Pieces = new List<Piece>();
            currentColor = "";
        }

        public void IncomingPiece(Piece piece)
        {
            // piece lands on field with several enemy pieces -> sent to start
            if (Pieces.Count > 1 && currentColor != piece.Color)
            {
                Debug.WriteLine($"{piece.Color} piece landed on field dominated by {currentColor} and is sent to start");
                piece.SendToStart();
            }
            // piece lands on field with sinle enemy piece -> sends enemy to start
            else if (Pieces.Count == 1 && currentColor != piece.Color)
            {
                Debug.WriteLine($"{piece.Color} piece landed on field with one {currentColor} piece which it sends to start");
                Pieces[0].SendToStart();
                Pieces.Remove(Pieces[0]);

                currentColor = piece.Color;
                Pieces.Add(piece);
            }
            // lands on empty field or field with own pieces
            else
            {
                currentColor = piece.Color;
                Pieces.Add(piece);
            }
        }

        public void OutgoingPiece(Piece piece)
        {
            Pieces.Remove(piece);
        }

        public void Print()
        {
            Console.Write("+----+");

            Console.Write("+----+");
        }

        public List<Piece> Pieces; // TODO set to private?

        string currentColor;

    }
}
