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
            setupWindow();
            setupElements();
            setupBorder();
        }

        // Setup elements ////////////////////////////////////////////////
        private void setupWindow()
        {
            // Window settings
            Width = 820;
            Height = 820;
            MaximizeBox = false;
            BackColor = GUI.GetColor("black");
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void setupElements()
        {
            // Winner screen
            winnerScreen = new EndScreen(this);

            // Open start screen / player select menu
            playerMenu = new PlayerSelectionMenu(this);

            // load in die
            GameDie = new Die(this);

            // load in dialog box
            Dialog = new DialogBox(this);

            foreach (var color in new List<string>() {"yellow", "blue", "red", "green" })
            {
                drawStartZone(color);
            }
        }

        private void setupBorder()
        {
            // create colored border
            int thickness = 20;
            border = new List<Label>();

            Label borderUpper = new Label();
            borderUpper.Location = new Point(0, 0);
            borderUpper.Size = new Size(820, thickness);
            border.Add(borderUpper);
            Controls.Add(borderUpper);

            Label borderLower = new Label();
            borderLower.Location = new Point(0, 781-thickness);
            borderLower.Size = new Size(820, thickness);
            border.Add(borderLower);
            Controls.Add(borderLower);

            Label borderLeft = new Label();
            borderLeft.Location = new Point(0, 0);
            borderLeft.Size = new Size(thickness, 820);
            border.Add(borderLeft);
            Controls.Add(borderLeft);

            Label borderRight = new Label();
            borderRight.Location = new Point(805-thickness, 0);
            borderRight.Size = new Size(thickness, 820);
            border.Add(borderRight);
            Controls.Add(borderRight);
        }

        private void drawStartZone(string color)
        {
            PictureBox startZone = new PictureBox();
            Controls.Add(startZone);

            startZone.Size = new Size(150, 150);
            startZone.Location = StartLocation(color);
            startZone.SizeMode = PictureBoxSizeMode.StretchImage;
            startZone.Image = (Image)Properties.Resources.ResourceManager.GetObject("start_" + color);

            startZone.Click += (object sender, EventArgs e) => { parent.StartClicked(color); };
        }

        public void ChangeBorderColor(string color)
        {
            foreach (var side in border)
            {
                side.BackColor = GUI.GetColor(color);
            }
        }

        // Input reactions /////////////////////////////////////////////
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
            parent.DieRolled();
        }

        public void FieldClicked(int index)
        {
            parent.MovePiece(index);
        }

        public void ShowEndScreen(Player winner)
        {
            winnerScreen.Display(winner.Color);
        }

        public void Replay()
        {
            parent.Reset();
        }

        // Static methods ////////////////////////////////////////////
        public static Color GetColor(string color)
        {
            switch (color)
            {
                case "red": return Color.Red;
                case "blue": return Color.DeepSkyBlue;
                case "yellow": return Color.Gold;
                case "green": return Color.ForestGreen;
                case "white": return Color.White;
                case "black": return Color.Black;

                default: return Color.Black;
            }
        }

        public static Point StartLocation(string color)
        {
            switch (color)
            {
                case "yellow":  return new Point(500, 100);
                case "blue": return new Point(550, 500);
                case "red": return new Point(150, 550);
                case "green": return new Point(100, 150);

                default: throw new Exception("Invalid input color");
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

                default: throw new Exception("Invalid field index");
            }
        }

        // Fields /////////////////////////////////////////////////////////
        List<Label> border;
        Game parent;
        PlayerSelectionMenu playerMenu;
        EndScreen winnerScreen;
        public Die GameDie;
        public DialogBox Dialog;
        
    }
}
