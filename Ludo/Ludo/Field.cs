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
        public Field(int i, GUI gui)
        {
            this.gui = gui;
            pieces = new List<Piece>();
            currentColor = "";
            index = i;

            setupLabel();
        }
        // Incoming / outgoing /////////////////////////////////////////////////
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

        public Piece GetPiece()
        {
            if (pieces.Count > 0)
            {
                // return first piece, its number is irrelevent
                return pieces[0];
            }
            else
            {
                return null;
            }
        }

        // Checking functions /////////////////////////////////////////////////////
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

        // Graphic functions ///////////////////////////////////////////////////////////
        public void OnClick(object sender, EventArgs e)
        {
            gui.FieldClicked(index);
        }
        private void setupLabel()
        {
            gui.Controls.Add(this);
            Width = 50;
            Height = 50;
            BorderStyle = BorderStyle.FixedSingle;
            Click += OnClick;

            //Text = index.ToString();

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
            else
            {
                BackColor = GUI.GetColor("white");
            }
        }

        public void Highlight(bool on)
        {
            /*
            if (on)
            {
                BorderStyle = BorderStyle.Fixed3D;
            }
            else
            {
                BorderStyle = BorderStyle.FixedSingle;
            }
            */
        }

        // Fields ///////////////////////////////////////////////////////////////////
        List<Piece> pieces;
        string currentColor;
        int index;
        GUI gui;
    }
}
