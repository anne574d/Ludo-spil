using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ludo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.SetWindowSize(20, 1);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Game game = new Game();
        }
    }
}