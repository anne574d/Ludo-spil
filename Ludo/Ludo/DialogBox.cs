using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace Ludo
{
    public class DialogBox : TextBox
    {
        public DialogBox(GUI parent)
        {
            parent.Controls.Add(this);
            Location = new Point(10, 620);
            Size = new Size(800, 290);
            Multiline = true;
            ReadOnly = true;

            border = new Label();
            parent.Controls.Add(border); 
            border.Location = new Point(0, 610);
            border.Size = new Size(820, 300);

            ChangeColor("black");
        }

        public void Print(string msg)
        {
            AppendText(msg + "\n");
        }
        public void ChangeColor(string color)
        {
            border.BackColor = Game.GetColor(color);
        }

        Label border;
    }

}
