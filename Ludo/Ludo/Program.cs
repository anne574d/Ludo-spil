using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ludo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Game game = new Game();

            Console.SetBufferSize(512, 512);

            for (int y = 0; y < 14; ++y)
            {
                for (int x = 0; x < 14; ++x)
                {
                    if (x == 0 || x == 13 || y == 0 || y == 13)
                    {
                        drawBox();
                    }
                    else
                    {
                        drawVoid();
                    }
                }
                newLine();
            }

            Console.Write("Exitted game. ");
            Console.ReadLine();
        }

        static void drawBox()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.Write("+----+ ");
            Console.SetCursorPosition(x, y + 1);
            Console.Write("|    | ");
            Console.SetCursorPosition(x, y + 2);
            Console.Write("|    | ");
            Console.SetCursorPosition(x, y + 3);
            Console.Write("+----+ ");

            Console.SetCursorPosition(x + 7, y);
        }

        static void drawVoid()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.SetCursorPosition(x + 7, y);
        }

        static void newLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop + 4);
        }
    }
}
