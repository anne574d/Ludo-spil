using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ludo
{
    class Piece
    {
        public Piece(string playerColor, int pieceNumber)
        {
            Color = playerColor;
            Number = pieceNumber;
            Position = -1;
            home = false;
        }

        // /////////////////////////////////////////////////////////////////
        // Movement functions //////////////////////////////////////////////
        // /////////////////////////////////////////////////////////////////
        public void Move(int diceroll)
        {
            if (diceroll <= 0)
            {
                // invalid diceroll;
            }
            // in start
            else if (Position == -1 && diceroll == 6)
            {
                Position = startExit();
            }
            // on home lane
            else if (Position >= homeLaneStart())
            {
                int sum = homeLaneEnd() - (Position + diceroll);
                Console.WriteLine($"sum = {sum}");
                if (sum == 0)
                {
                    // in goal/home
                    Console.WriteLine("HOME");
                    home = true;
                }
                else if (sum > 0)
                {
                    // doesn't reach home
                    Position += diceroll;
                }
                else
                {
                    // bounce back from home
                    int newPos = 2 * homeLaneEnd() - (Position + diceroll);
                    Console.WriteLine($"New position: {newPos}");
                    Position = 2 * homeLaneEnd() - (Position + diceroll);
                }
            }
            else
            {
                // in outer ring
                for (int step = 1; step <= diceroll; ++step)
                {
                    Position++;
                    if (Position == 52)
                    {
                        // wrap around
                        Position = 0;
                    }
                    else if (Position == homeLaneEntry() + 1)
                    {
                        Position = homeLaneStart();
                        Move(diceroll - step);
                        break;
                    }
                }
            }
        }

        public void SendToStart()
        {
            Position = -1;
        }

        // /////////////////////////////////////////////////////////////////
        // Checking functions //////////////////////////////////////////////
        // /////////////////////////////////////////////////////////////////
        public bool IsHome()
        {
            return home;
        }

        public bool Overtakes(Piece p, int diceroll)
        {
            // TODO: Find a way that check in the 51->0 border
            bool res;
            if (p == this)
            {
                // doesn't overtake itself
                res = false;
            }
            else if (p.Position == -1)
            {
                // cannot overtake someone in start
                res = false;
            }
            else
            {
                res = (Position < p.Position && Position + diceroll > p.Position);
                Debug.WriteLine($"{res}: {Color}{Number} as pos {Position} overtakes {p.Color}{p.Number} at pos {p.Position} (roll: {diceroll})");
            }
            return res;
        }

        public bool CanLeaveStart(int diceroll)
        {
            return (Position == -1 && diceroll == 6);
        }

            // /////////////////////////////////////////////////////////////////
            // Color specific positions ////////////////////////////////////////
            // /////////////////////////////////////////////////////////////////
            public int startExit()
        {
            int result;
            switch (Color)
            {
                case "yellow": result = 0; break;
                case "blue": result = 13; break;
                case "red": result = 26; break;
                case "green": result = 39; break;
                default: result = 0; break;
            }
            return result;
        }
        private int homeLaneEntry()
        {
            int result;
            switch (Color)
            {
                case "yellow": result = 50; break;
                case "blue": result = 11; break;
                case "red": result = 24; break;
                case "green": result = 37; break;
                default: result = 0; break;
            }
            return result;
        }
        private int homeLaneStart()
        {
            int result;
            switch (Color)
            {
                case "yellow": result = 52; break;
                case "blue": result = 58; break;
                case "red": result = 64; break;
                case "green": result = 70; break;
                default: result = 0; break;
            }
            return result;
        }
        private int homeLaneEnd()
        {
            int result;
            switch (Color)
            {
                case "yellow": result = 57; break;
                case "blue": result = 63; break;
                case "red": result = 69; break;
                case "green": result = 75; break;
                default: result = 0; break;
            }
            return result;
        }

        public void DebugPrint()
        {
            Debug.WriteLine($"Piece = {Color}{Number} // Position = {Position}");
        }

        // Publics //////////////////////////////
        public int Position, Number;
        public string Color { get; private set; }

        // Privates /////////////////////////////
        bool home;
    }
}
