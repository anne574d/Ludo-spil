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
    public class Field : Label
    {
        public Field(int i, GUI parent)
        {
            this.parent = parent;
            pieces = new List<Piece>();
            currentColor = "";
            index = i;

            setupLabel();
        }
        // Incoming / outgoing ///////////////////////////////////////////
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
        public Piece OutgoingPiece()
        {
            Piece p = pieces[0];
            pieces.RemoveAt(0);
            return p;
        }

        // Checking functions ///////////////////////////////////////////
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

        // Graphic functions ///////////////////////////////////////////
        private void onClick(object sender, EventArgs e)
        {
            parent.FieldClicked(index);
        }
        private void setupLabel()
        {
            Parent = parent;
            Width = 50;
            Height = 50;
            BorderStyle = BorderStyle.Fixed3D;
            Click += onClick;

            Text = index.ToString(); // todo remove

            Location = GUI.FieldLocation(index);
            colorLabel();
        }
        private void colorLabel()
        {
            if ((index >= 52 && index <= 57) || index == 0)
            {
                BackColor = GUI.GetColor("yellow");
            }
            else if ((index >= 58 && index <= 63) || index == 13)
            {
                BackColor = GUI.GetColor("blue");
            }
            else if ((index >= 64 && index <= 69) || index == 26)
            {
                BackColor = GUI.GetColor("red");
            }
            else if ((index >= 70 && index <= 75) || index == 39)
            {
                BackColor = GUI.GetColor("green");
            }
        }

        public void Highlight(bool on)
        {
            if (on)
            {
                BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                BorderStyle = BorderStyle.Fixed3D;
            }
        }


        /*
        private void positionLabel()
        {
            switch (index)
            {
                case 0: Location = new Point(400, 100); break;
                case 1: Location = new Point(400, 150); break;
                case 2: Location = new Point(400, 200); break;
                case 3: Location = new Point(400, 250); break;
                case 4: Location = new Point(400, 300); break;
                case 5: Location = new Point(450, 300); break;
                case 6: Location = new Point(500, 300); break;
                case 7: Location = new Point(550, 300); break;
                case 8: Location = new Point(600, 300); break;
                case 9: Location = new Point(650, 300); break;
                case 10: Location = new Point(700, 300); break;
                case 11: Location = new Point(700, 350); break;
                case 12: Location = new Point(700, 400); break;
                case 13: Location = new Point(650, 400); break;
                case 14: Location = new Point(600, 400); break;
                case 15: Location = new Point(550, 400); break;
                case 16: Location = new Point(500, 400); break;
                case 17: Location = new Point(450, 400); break;
                case 18: Location = new Point(450, 450); break;
                case 19: Location = new Point(450, 500); break;
                case 20: Location = new Point(450, 550); break;
                case 21: Location = new Point(450, 600); break;
                case 22: Location = new Point(450, 650); break;
                case 23: Location = new Point(450, 700); break;
                case 24: Location = new Point(400, 700); break;
                case 25: Location = new Point(350, 700); break;
                case 26: Location = new Point(350, 650); break;
                case 27: Location = new Point(350, 600); break;
                case 28: Location = new Point(350, 550); break;
                case 29: Location = new Point(350, 500); break;
                case 30: Location = new Point(350, 450); break;
                case 31: Location = new Point(300, 450); break;
                case 32: Location = new Point(250, 450); break;
                case 33: Location = new Point(200, 450); break;
                case 34: Location = new Point(150, 450); break;
                case 35: Location = new Point(100, 450); break;
                case 36: Location = new Point(50, 450); break;
                case 37: Location = new Point(50, 400); break;
                case 38: Location = new Point(50, 350); break;
                case 39: Location = new Point(100, 350); break;
                case 40: Location = new Point(150, 350); break;
                case 41: Location = new Point(200, 350); break;
                case 42: Location = new Point(250, 350); break;
                case 43: Location = new Point(300, 350); break;
                case 44: Location = new Point(300, 300); break;
                case 45: Location = new Point(300, 250); break;
                case 46: Location = new Point(300, 200); break;
                case 47: Location = new Point(300, 150); break;
                case 48: Location = new Point(300, 100); break;
                case 49: Location = new Point(300, 50); break;
                case 50: Location = new Point(350, 50); break;
                case 51: Location = new Point(400, 50); break;

                case 52: Location = new Point(350, 100); break;
                case 53: Location = new Point(350, 150); break;
                case 54: Location = new Point(350, 200); break;
                case 55: Location = new Point(350, 250); break;
                case 56: Location = new Point(350, 300); break;
                case 57: Location = new Point(350, 350); break;

                case 58: Location = new Point(650, 350); break;
                case 59: Location = new Point(600, 350); break;
                case 60: Location = new Point(550, 350); break;
                case 61: Location = new Point(500, 350); break;
                case 62: Location = new Point(450, 350); break;
                case 63: Location = new Point(400, 350); break;

                case 64: Location = new Point(400, 650); break;
                case 65: Location = new Point(400, 600); break;
                case 66: Location = new Point(400, 550); break;
                case 67: Location = new Point(400, 500); break;
                case 68: Location = new Point(400, 450); break;
                case 69: Location = new Point(400, 400); break;

                case 70: Location = new Point(100, 400); break;
                case 71: Location = new Point(150, 400); break;
                case 72: Location = new Point(200, 400); break;
                case 73: Location = new Point(250, 400); break;
                case 74: Location = new Point(300, 400); break;
                case 75: Location = new Point(350, 400); break;
            }
        }*/

        List<Piece> pieces;
        string currentColor;
        int index;
        GUI parent;
    }
}
