﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Ludo
{
    class PlayerSelectionMenu : GroupBox
    {
        public PlayerSelectionMenu(Form parent)
        {
            Parent = parent;
            Size = new Size(820, 820);
            Location = new Point(0, 0);

            addTitle();
            addStart();
            addColors();

            Hide();
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
            title.BackColor = Color.Black;
            title.ForeColor = Color.White;

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
            start.BackColor = Color.Black;
            start.ForeColor = Color.White;

            start.Click += startClicked;
        }

        private void addColors()
        {
            List<string> colors = new List<string> { "green", "yellow", "blue", "red" };
            int xPos = 0;

            foreach (var col in colors)
            {
                PlayerSelect lbl = new PlayerSelect(col);
                Controls.Add(lbl);
                lbl.MoveHorizontally(xPos);

                xPos += 205;
            }
        }

        private void startClicked(object sender, EventArgs e)
        {
            foreach (PlayerSelect ps in Controls.OfType<PlayerSelect>())
            {
                if (ps.IsActive())
                {
                    Debug.WriteLine($"{ps.GetColor()} player is human: {ps.IsHuman()}");
                }
            }
            Debug.WriteLine("Start!");
            Hide();
        }
    }

    public class PlayerSelect : Label
    {
        public PlayerSelect(string color)
        {
            playerColor = color;

            BackColor = Game.GetColor(color);
            Size = new Size(205, 820);
            Text = "";
            TextAlign = ContentAlignment.MiddleCenter;
            Font = new Font("Arial", 24, FontStyle.Bold);

            Click += clickEvent;
        }

        public void MoveHorizontally(int x)
        {
            Location = new Point(x,0);
        }
        private void clickEvent(object sender, EventArgs e)
        {
            switch (Text)
            {
                case "": Text = "PLAYER"; break;
                case "PLAYER": Text = "COM"; break;
                case "COM": Text = ""; break;

                default: Text = ""; break;
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

        string playerColor;
    }
}
