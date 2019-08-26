using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Ludo
{
    public partial class GUI : Form
    {
        public GUI()
        {
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

            PlayerSelectionMenu playerMenu = new PlayerSelectionMenu(this);

            // Game settings
            game = new Game();
            game.SetupBoard(this);
        }
        private void updateBoard()
        {
            foreach (var player in game.Players)
            {
                foreach (var piece in player.Pieces)
                {

                }
            }
        }

        Game game;
        GroupBox playerSelectMenu;

    }
}
