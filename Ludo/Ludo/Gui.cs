using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ludo
{
    public partial class GUI : Form
    {
        public GUI(Game parent)
        {
            this.parent = parent;

            InitializeComponent();
            setup();
        }

        private void setup()
        {
            // Window settings
            Width = 820;
            Height = 820;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            // Open start screen / player select menu
            playerMenu = new PlayerSelectionMenu(this);
            Controls.Add(playerMenu);
            playerMenu.Show();

            // load in die
            GameDie = new Die(this);
            
            // load in dialog
            Dialog = new DialogBox(this);

            Dialog.Print("Black hello");
            Dialog.ChangeColor("blue");
            Dialog.Print("Blue hello");
        }

        public void ExitPlayerSelectionMenu()
        {
            foreach (var player in playerMenu.GetPlayers())
            {
                parent.AddPlayer(player.GetColor(), player.IsHuman());
            }
            parent.StartGame();
        }

        public void DieRolled()
        {
            parent.NextTurn();
        }

        public void FieldClicked(int index)
        {
            parent.MovePiece(index);
        }

        public void UpdateBoard(Field[] board)
        {
            for (int i = 0; i < board.Length; ++i)
            {
                board[i].Highlight(false); // remove previous turns highlight
            }
            // todo draw pieces at start
        }

        public void ShowEndScreen(Player winner)
        {
            Debug.Write($"{winner.Color} WINS!");
        }


        public static Color GetColor(string color)
        {
            switch (color)
            {
                case "red": return Color.Red;
                case "blue": return Color.DeepSkyBlue;
                case "yellow": return Color.Gold;
                case "green": return Color.Green;

                default: return Color.Black;
            }
        }

        public static Point StartLocation(string color)
        {
            switch (color)
            {
                case "yellow":  return
            }
        }
        public static Point FieldLocation(int i)
        {
            switch (i)
            {
                case 0: return new Point(400, 100);
                case 1: return new Point(400, 150);
                case 2: return new Point(400, 200);
                case 3: return new Point(400, 250);
                case 4: return new Point(400, 300);
                case 5: return new Point(450, 300);
                case 6: return new Point(500, 300);
                case 7: return new Point(550, 300);
                case 8: return new Point(600, 300);
                case 9: return new Point(650, 300);
                case 10: return new Point(700, 300);
                case 11: return new Point(700, 350);
                case 12: return new Point(700, 400);
                case 13: return new Point(650, 400);
                case 14: return new Point(600, 400);
                case 15: return new Point(550, 400);
                case 16: return new Point(500, 400);
                case 17: return new Point(450, 400);
                case 18: return new Point(450, 450);
                case 19: return new Point(450, 500);
                case 20: return new Point(450, 550);
                case 21: return new Point(450, 600);
                case 22: return new Point(450, 650);
                case 23: return new Point(450, 700);
                case 24: return new Point(400, 700);
                case 25: return new Point(350, 700);
                case 26: return new Point(350, 650);
                case 27: return new Point(350, 600);
                case 28: return new Point(350, 550);
                case 29: return new Point(350, 500);
                case 30: return new Point(350, 450);
                case 31: return new Point(300, 450);
                case 32: return new Point(250, 450);
                case 33: return new Point(200, 450);
                case 34: return new Point(150, 450);
                case 35: return new Point(100, 450);
                case 36: return new Point(50, 450);
                case 37: return new Point(50, 400);
                case 38: return new Point(50, 350);
                case 39: return new Point(100, 350);
                case 40: return new Point(150, 350);
                case 41: return new Point(200, 350);
                case 42: return new Point(250, 350);
                case 43: return new Point(300, 350);
                case 44: return new Point(300, 300);
                case 45: return new Point(300, 250);
                case 46: return new Point(300, 200);
                case 47: return new Point(300, 150);
                case 48: return new Point(300, 100);
                case 49: return new Point(300, 50);
                case 50: return new Point(350, 50);
                case 51: return new Point(400, 50);

                case 52: return new Point(350, 100);
                case 53: return new Point(350, 150);
                case 54: return new Point(350, 200);
                case 55: return new Point(350, 250);
                case 56: return new Point(350, 300);
                case 57: return new Point(350, 350);

                case 58: return new Point(650, 350);
                case 59: return new Point(600, 350);
                case 60: return new Point(550, 350);
                case 61: return new Point(500, 350);
                case 62: return new Point(450, 350);
                case 63: return new Point(400, 350);

                case 64: return new Point(400, 650);
                case 65: return new Point(400, 600);
                case 66: return new Point(400, 550);
                case 67: return new Point(400, 500);
                case 68: return new Point(400, 450);
                case 69: return new Point(400, 400);

                case 70: return new Point(100, 400);
                case 71: return new Point(150, 400);
                case 72: return new Point(200, 400);
                case 73: return new Point(250, 400);
                case 74: return new Point(300, 400);
                case 75: return new Point(350, 400);

                default: return new Point(0, 0);
            }
        }


        Game parent;
        PlayerSelectionMenu playerMenu;
        public Die GameDie;
        public DialogBox Dialog;
    }
}
