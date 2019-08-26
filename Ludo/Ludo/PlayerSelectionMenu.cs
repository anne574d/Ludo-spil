using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Ludo
{
    class PlayerSelectionMenu : GroupBox
    {
        public PlayerSelectionMenu(Form parent)
        {
            Parent = parent;
            Size = new Size(820, 820);
            Location = new Point(0, 0);

            addColors();

            //Hide();
        }

        private void addColors()
        {
            List<string> colors = new List<string> { "green", "yellow", "blue", "red" };
            int xPos = 0;

            foreach (var col in colors)
            {
                Label lbl = new Label();
                Controls.Add(lbl);

                lbl.BackColor = Game.GetColor(col);
                lbl.Size = new Size(205, 820);
                lbl.Location = new Point(xPos, 0);
                lbl.Text = "";

                lbl.Click += (sender, EventArgs) => { colorClicked(sender, EventArgs, lbl); };

                xPos += 205;
            }
        }

        private void colorClicked(object sender, EventArgs e, Label lbl)
        {
            switch (lbl.Text)
            {
                case "": lbl.Text = "PLAYER"; break;
                case "PLAYER": lbl.Text = "COM"; break;
                case "COM": lbl.Text = ""; break;

                default: lbl.Text = ""; break;
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }
    }
}
