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
            Location = new Point(20, 20);
            Size = new Size(280, 100);
            Multiline = true;
            ReadOnly = true;
            BorderStyle = BorderStyle.None;
            Font = new Font("Arial", 12, FontStyle.Regular);
        }

        public void Write(string msg)
        {
            AppendText(msg);
        }
        public void WriteLine(string msg)
        {
            AppendText(msg + "\n");
        }
    }

}
