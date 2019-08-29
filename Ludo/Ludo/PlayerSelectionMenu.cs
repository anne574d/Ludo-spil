using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Ludo
{
    public class PlayerSelectionMenu : Panel
    {
        public PlayerSelectionMenu(GUI parent)
        {
            this.parent = parent;
            parent.Controls.Add(this);
            Size = new Size(820, 820);
            Location = new Point(0, 0);

            addTitle();
            addStart();
            addColors();
        }

        private void addTitle()
        {
            Label title = new Label();
            Controls.Add(title);
            title.Size = new Size(820, 200);
            title.Location = new Point(0, 0);
            title.Text = "L U D O";
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Font = new Font("Arial", 100, FontStyle.Bold);
            title.BackColor = GUI.GetColor("black");
            title.ForeColor = GUI.GetColor("white");

            title.Click += (sender, e) => { System.Environment.Exit(0); };
        }

        private void addStart()
        {
            Label start = new Label();
            Controls.Add(start);
            start.Size = new Size(820, 200);
            start.Location = new Point(0, 585);
            start.Text = "S T A R T";
            start.TextAlign = ContentAlignment.MiddleCenter;
            start.Font = new Font("Arial", 100, FontStyle.Bold);
            start.BackColor = GUI.GetColor("black");
            start.ForeColor = GUI.GetColor("white");

            start.Click += startClicked;
        }

        private void addColors()
        {
            List<string> colors = new List<string> { "green", "yellow", "blue", "red" };

            for(int i = 0; i < colors.Count; ++i)
            {
                PlayerSelect lbl = new PlayerSelect(colors[i], 205 * i);
                Controls.Add(lbl);
            }
        }

        private int activePlayers()
        {
            int count = 0;
            foreach (PlayerSelect ps in Controls.OfType<PlayerSelect>())
            {
                if (ps.IsActive())
                {
                    ++count;
                }
            }
            return count;
        }

        private void startClicked(object sender, EventArgs e)
        {
            if (activePlayers() >= 2)
            {
                Hide();
                parent.ExitPlayerSelectionMenu();
            }
        }

        public List<PlayerSelect> GetPlayers()
        {
            List<PlayerSelect> result = new List<PlayerSelect>();
            foreach (PlayerSelect player in Controls.OfType<PlayerSelect>())
            {
                if (player.IsActive())
                {
                    result.Add(player);
                }
            }
            return result;
        }

        GUI parent;
    }

    public class PlayerSelect : Label
    {
        public PlayerSelect(string color, int xPos)
        {
            playerColor = color;

            BackColor = GUI.GetColor(color);
            Size = new Size(205, 820);
            Location = new Point(xPos, 0);
            Text = "";
            TextAlign = ContentAlignment.MiddleCenter;
            Font = new Font("Arial", 24, FontStyle.Bold);

            Click += onClick;
        }

        private void onClick(object sender, EventArgs e)
        {
            switch (Text)
            {
                case "": Text = "PLAYER"; break;
                case "PLAYER": Text = "COM"; break;
                case "COM": Text = ""; break;

                default: throw new Exception($"Unknown player state in player selection menu: \"{Text}\"");
            }
        }
        public string GetColor()
        {
            return playerColor;
        }
        public bool IsHuman()
        {
            return (Text == "PLAYER");
        }
        public bool IsActive()
        {
            return (Text != "");
        }

        // Fields
        string playerColor;
    }
}
