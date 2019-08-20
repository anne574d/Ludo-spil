using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ludo
{
    class Printer
    {
        public Printer()
        {
            Console.SetWindowSize(74, 48);
            Console.SetBufferSize(74, 148);
            ChangeFontColor("default");
            Console.CursorVisible = false;
            Console.Clear();
        }

        // ////////////////////////////////////
        // Drawing ////////////////////////////
        // ////////////////////////////////////
        public void PrintBoard()
        {
            Console.Clear();
            // draw outer ring
            drawBoxes(false);
            // draw colored homelanes
            // (colored lines overwrite black ones)
            drawBoxes(true);

            // move cursor to write area
            moveCursorOutsideBoard();
        }

        public void UpdateBoard(List<Player> players)
        {
            // remove all pieces on board
            for (int i = 0; i < 76; ++i)
            {
                moveToField(i);
                clearField();
            }

            foreach (var player in players)
            {
                // removes piece inside start
                moveToStart(player.Color);
                clearField();

                // draw each players pieces at their position
                foreach (var piece in player.Pieces)
                {
                    if (piece.IsAtStart() )
                    {
                        moveToStart(player.Color);
                    }
                    else
                    {
                        moveToField(piece.Position);
                    }
                    placePiece(piece);
                }
            }
            clearDialog();
        }

        private void drawBoxes(bool colored)
        {
            Console.SetCursorPosition(2, 1);
            for (int y = 0; y < 14; ++y)
            {
                for (int x = 0; x < 14; ++x)
                {
                    if (boxAtPosition(x, y, colored))
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
        }

        private bool boxAtPosition(int x, int y, bool colored)
        {
            bool res = false;

            if ((x == 6 && y > 0 && y < 7) || // yellow homelane
                (x == 8 && y == 0) )          // yellow start
            {
                ChangeFontColor("yellow");
                res = colored;
            }
            else if ((y == 6 && x > 6 && x < 13) || // blue homelane
                     (x == 13 && y == 8) )          // blue start
            {
                ChangeFontColor("blue");
                res = colored;
            }
            else if ((x == 7 && y > 6 && y < 13) || // red homelane
                     (x == 5 && y == 13))           // red start
            {
                ChangeFontColor("red");
                res = colored;
            }
            else if ((y == 7 && x > 0 && x < 7) || // green homelane
                     (x == 0 && y == 5))           // green start         
            {
                ChangeFontColor("green");
                res = colored;
            }
            else if (x == 0 || y == 0 || x == 13 || y == 13) // outer ring
            {
                ChangeFontColor("black");
                res = !colored;
            }
            return res;
        }

        private void drawBox()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.Write("+----+");
            Console.SetCursorPosition(x, y + 1);
            Console.Write("|    |");
            Console.SetCursorPosition(x, y + 2);
            Console.Write("|    |");
            Console.SetCursorPosition(x, y + 3);
            Console.Write("+----+");

            Console.SetCursorPosition(x + 5, y);
        }

        private void drawVoid()
        {
            // mimics the end position of drawBox() without writing anything
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.SetCursorPosition(x + 5, y);
        }

        private void clearField()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.Write("  ");
            Console.SetCursorPosition(x, y + 1);
            Console.Write("  ");
            Console.SetCursorPosition(x, y);
        }
        private void clearDialog()
        {
            ChangeFontColor("black");
            moveCursorOutsideBoard();
            Console.Write("                                                                                                            ");
            Console.Write("                                                                                                            ");
            Console.Write("                                                                                                            ");
            Console.SetCursorPosition(0, 0); // move to top to ensure the entire board is in view
            moveCursorOutsideBoard();
        }

        public static void ClearKeyBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
        }

        private void placePiecesOnField(Field field)
        {
            foreach (var piece in field.Pieces)
            {
                placePiece(piece);
            }
        }

        private void placePiecesAtStart(Player player)
        {
            moveToStart(player.Color);
            foreach (var piece in player.Pieces)
            {
                if (piece.IsAtStart())
                {
                    placePiece(piece);
                }
            }
        }
        private void placePiece(Piece piece)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            ChangeFontColor(piece.Color + "-piece");
            switch(piece.Number)
            {
                case 1:
                    Console.SetCursorPosition(x, y);
                    Console.Write("1");
                    break;
                case 2:
                    Console.SetCursorPosition(x + 1, y);
                    Console.Write("2");
                    break;
                case 3:
                    Console.SetCursorPosition(x, y + 1);
                    Console.Write("3");
                    break;
                case 4:
                    Console.SetCursorPosition(x + 1, y + 1);
                    Console.Write("4");
                    break;
            }
            Console.SetCursorPosition(x, y);
            ChangeFontColor("default");
        }

        public void PrintStartScreen()
        {
            ClearKeyBuffer();
            Console.Clear();
            Console.WriteLine(@"
    Welcome to... 
     __        __    __  _______    ______  
    /  |      /  |  /  |/       \  /      \ 
    $$ |      $$ |  $$ |$$$$$$$  |/$$$$$$  |
    $$ |      $$ |  $$ |$$ |  $$ |$$ |  $$ |
    $$ |      $$ |  $$ |$$ |  $$ |$$ |  $$ |
    $$ |      $$ |  $$ |$$ |  $$ |$$ |  $$ |
    $$ |_____ $$ \__$$ |$$ |__$$ |$$ \__$$ |
    $$       |$$    $$/ $$    $$/ $$    $$/ 
    $$$$$$$$/  $$$$$$/  $$$$$$$/   $$$$$$/  
                                        
    Press ENTER to start game or ESC to quit
            ");
        }

        public void PrintEndScreen(Player winner)
        {
            ClearKeyBuffer();
            Console.Clear();
            ChangeFontColor(winner.Color);

            switch (winner.Color)
            {
                case "yellow": Console.WriteLine(@"
   __   __ ___  _     _     ___ __      __ __      __ ___  _  _  ___ 
   \ \ / /| __|| |   | |   / _ \\ \    / / \ \    / /|_ _|| \| |/ __|
    \ V / | _| | |__ | |__| (_) |\ \/\/ /   \ \/\/ /  | | | .` |\__ \
     |_|  |___||____||____|\___/  \_/\_/     \_/\_/  |___||_|\_||___/
                "); break;

                case "blue": Console.WriteLine(@"
    ___  _    _   _  ___  __      __ ___  _  _  ___ 
   | _ )| |  | | | || __| \ \    / /|_ _|| \| |/ __|
   | _ \| |__| |_| || _|   \ \/\/ /  | | | .` |\__ \
   |___/|____|\___/ |___|   \_/\_/  |___||_|\_||___/
                "); break;

                case "red": Console.WriteLine(@"
    ___  ___  ___   __      __ ___  _  _  ___ 
   | _ \| __||   \  \ \    / /|_ _|| \| |/ __|
   |   /| _| | |) |  \ \/\/ /  | | | .` |\__ \
   |_|_\|___||___/    \_/\_/  |___||_|\_||___/
                "); break;

                case "green": Console.WriteLine(@"
     ___  ___  ___  ___  _  _  __      __ ___  _  _  ___ 
    / __|| _ \| __|| __|| \| | \ \    / /|_ _|| \| |/ __|
   | (_ ||   /| _| | _| | .` |  \ \/\/ /  | | | .` |\__ \
    \___||_|_\|___||___||_|\_|   \_/\_/  |___||_|\_||___/
                "); break;
                
                default: break;
            }
            Console.WriteLine("   Press ENTER to continue \n\n");
            Console.ReadLine();
        }


        // //////////////////////////////////////
        // Move cursor //////////////////////////
        // //////////////////////////////////////
        private void newLine()
        {
            Console.SetCursorPosition(2, Console.CursorTop + 3);
        }

        private void moveCursorOutsideBoard()
        {
            Console.SetCursorPosition(0, 45);
        }

        private void moveToField(int field)
        {
            // cursor coordinates for all fields
            // 0 - 51 is outer ring
            // 52 - 75 is home lanes
            switch (field)
            {
                default: moveCursorOutsideBoard(); break;

                case 0: Console.SetCursorPosition(44, 2); break;
                case 1: Console.SetCursorPosition(49, 2); break;
                case 2: Console.SetCursorPosition(54, 2); break;
                case 3: Console.SetCursorPosition(59, 2); break;
                case 4: Console.SetCursorPosition(64, 2); break;
                case 5: Console.SetCursorPosition(69, 2); break;
                case 6: Console.SetCursorPosition(69, 5); break;
                case 7: Console.SetCursorPosition(69, 8); break;
                case 8: Console.SetCursorPosition(69, 11); break;
                case 9: Console.SetCursorPosition(69, 14); break;
                case 10: Console.SetCursorPosition(69, 17); break;
                case 11: Console.SetCursorPosition(69, 20); break;
                case 12: Console.SetCursorPosition(69, 23); break;
                case 13: Console.SetCursorPosition(69, 26); break;
                case 14: Console.SetCursorPosition(69, 29); break;
                case 15: Console.SetCursorPosition(69, 32); break;
                case 16: Console.SetCursorPosition(69, 35); break;
                case 17: Console.SetCursorPosition(69, 38); break;
                case 18: Console.SetCursorPosition(69, 41); break;
                case 19: Console.SetCursorPosition(64, 41); break;
                case 20: Console.SetCursorPosition(59, 41); break;
                case 21: Console.SetCursorPosition(54, 41); break;
                case 22: Console.SetCursorPosition(49, 41); break;
                case 23: Console.SetCursorPosition(44, 41); break;
                case 24: Console.SetCursorPosition(39, 41); break;
                case 25: Console.SetCursorPosition(34, 41); break;
                case 26: Console.SetCursorPosition(29, 41); break;
                case 27: Console.SetCursorPosition(24, 41); break;
                case 28: Console.SetCursorPosition(19, 41); break;
                case 29: Console.SetCursorPosition(14, 41); break;
                case 30: Console.SetCursorPosition(9, 41); break;
                case 31: Console.SetCursorPosition(4, 41); break;
                case 32: Console.SetCursorPosition(4, 38); break;
                case 33: Console.SetCursorPosition(4, 35); break;
                case 34: Console.SetCursorPosition(4, 32); break;
                case 35: Console.SetCursorPosition(4, 29); break;
                case 36: Console.SetCursorPosition(4, 26); break;
                case 37: Console.SetCursorPosition(4, 23); break;
                case 38: Console.SetCursorPosition(4, 20); break;
                case 39: Console.SetCursorPosition(4, 17); break;
                case 40: Console.SetCursorPosition(4, 14); break;
                case 41: Console.SetCursorPosition(4, 11); break;
                case 42: Console.SetCursorPosition(4, 8); break;
                case 43: Console.SetCursorPosition(4, 5); break;
                case 44: Console.SetCursorPosition(4, 2); break;
                case 45: Console.SetCursorPosition(9, 2); break;
                case 46: Console.SetCursorPosition(14, 2); break;
                case 47: Console.SetCursorPosition(19, 2); break;
                case 48: Console.SetCursorPosition(24, 2); break;
                case 49: Console.SetCursorPosition(29, 2); break;
                case 50: Console.SetCursorPosition(34, 2); break;
                case 51: Console.SetCursorPosition(39, 2); break;

                case 52: Console.SetCursorPosition(34, 5); break;
                case 53: Console.SetCursorPosition(34, 8); break;
                case 54: Console.SetCursorPosition(34, 11); break;
                case 55: Console.SetCursorPosition(34, 14); break;
                case 56: Console.SetCursorPosition(34, 17); break;
                case 57: Console.SetCursorPosition(34, 20); break;

                case 58: Console.SetCursorPosition(64, 20); break;
                case 59: Console.SetCursorPosition(59, 20); break;
                case 60: Console.SetCursorPosition(54, 20); break;
                case 61: Console.SetCursorPosition(49, 20); break;
                case 62: Console.SetCursorPosition(44, 20); break;
                case 63: Console.SetCursorPosition(39, 20); break;

                case 64: Console.SetCursorPosition(39, 38); break;
                case 65: Console.SetCursorPosition(39, 35); break;
                case 66: Console.SetCursorPosition(39, 32); break;
                case 67: Console.SetCursorPosition(39, 29); break;
                case 68: Console.SetCursorPosition(39, 26); break;
                case 69: Console.SetCursorPosition(39, 23); break;

                case 70: Console.SetCursorPosition(9, 23); break;
                case 71: Console.SetCursorPosition(14, 23); break;
                case 72: Console.SetCursorPosition(19, 23); break;
                case 73: Console.SetCursorPosition(24, 23); break;
                case 74: Console.SetCursorPosition(29, 23); break;
                case 75: Console.SetCursorPosition(34, 23); break;
            }
        }

        private void moveToStart(string color)
        {
            switch (color)
            {
                // "Fields" outside board, right next to startExit(),
                // to show how many pieces are still inside start. 
                case "yellow": Console.SetCursorPosition(44, 5); break;
                case "blue": Console.SetCursorPosition(63, 26); break;
                case "red": Console.SetCursorPosition(29, 38); break;
                case "green": Console.SetCursorPosition(9, 17); break;
            }
        }


        // ////////////////////////////////////////
        // Colors /////////////////////////////////
        // ////////////////////////////////////////

        public static void ChangeFontColor(string color)
        {
            switch (color)
            {
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "yellow-piece":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "blue-piece":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "red-piece":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case "green":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "green-piece":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}