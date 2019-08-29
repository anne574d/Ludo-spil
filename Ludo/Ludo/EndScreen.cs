using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace Ludo
{
    public class EndScreen : Panel
    {
        public EndScreen(GUI parent)
        {
            this.parent = parent;
            parent.Controls.Add(this);

            Size = new Size(820, 820);
            Location = new Point(0, 0);
            BorderStyle = BorderStyle.None;

            winner = new Label();
            Controls.Add(winner);
            winner.Size = new Size(820, 200);
            winner.Location = new Point(0, 290);
            winner.TextAlign = ContentAlignment.MiddleCenter;
            winner.Font = new Font("Arial", 70, FontStyle.Bold);
            winner.BackColor = GUI.GetColor("black");
            winner.ForeColor = GUI.GetColor("white");

            replay = new Label();
            Controls.Add(replay);
            replay.Size = new Size(820, 200);
            replay.Location = new Point(0, 50);
            replay.TextAlign = ContentAlignment.MiddleCenter;
            replay.Font = new Font("Arial", 100, FontStyle.Bold);
            replay.Text = "R E P L A Y";
            replay.ForeColor = GUI.GetColor("black");
            replay.Click += (sender, e) => { parent.Replay(); };  // todo

            quit = new Label();
            Controls.Add(quit);
            quit.Size = new Size(820, 200);
            quit.Location = new Point(0, 520);
            quit.TextAlign = ContentAlignment.MiddleCenter;
            quit.Font = new Font("Arial", 100, FontStyle.Bold);
            quit.Text = "Q U I T";
            quit.ForeColor = GUI.GetColor("black");
            quit.Click += (sender, e) => { System.Environment.Exit(0); };

            Hide();
        }

        public void Display(string color)
        {
            BackColor = GUI.GetColor(color);
            replay.BackColor = GUI.GetColor(color);
            quit.BackColor = GUI.GetColor(color);

            BringToFront();

            winner.Text = $"{color.ToUpper()} WINS!";
            Show();
        }

        Label winner, replay, quit;
        GUI parent;
    }
}
