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
            IsHome = false;
            createRoute();
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
            // inside start
            else if (IsAtStart() && diceroll == 6)
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
                    Position = homeLaneEnd();
                    IsHome = true;
                }
                else if (sum > 0)
                {
                    // doesn't reach home
                    Position += diceroll;
                }
                else
                {
                    // bounce back from home
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
                        // enter homelane
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
        public bool Overtakes(Piece p, int diceroll)
        {
            // Determine whether a piece overtakes one of its own
            bool res;
            if (p == this || p.Position == -1 || p.IsHome )
            {
                // Does not overtake itself, someone inside start or someone who has reached home 
                res = false;
            }
            else
            {
                // Compare position on "route", avoids problems at 51->0 and in homelane
                res = (route.IndexOf(Position) < route.IndexOf(p.Position) && route.IndexOf(Position) + diceroll > route.IndexOf(p.Position));
            }
            return res;
        }

        public bool CanLeaveStart(int diceroll)
        {
            return (Position == -1 && diceroll == 6);
        }

        public bool IsAtStart()
        {
            return (Position == -1);
        }

        // /////////////////////////////////////////////////////////////////
        // Color specific positions ////////////////////////////////////////
        // /////////////////////////////////////////////////////////////////

        private void createRoute()
        {
            /* Create route called in constructor.
               Route is used to ensure a piece cannot 
               overtake pieces with its own color */

            route = new List<int>();

            int i = startExit();
            while (i <= homeLaneEnd())
            {
                if (i == homeLaneEntry() + 1)
                {
                    i = homeLaneStart();
                    route.Add(i);
                }
                else if (i == 51)
                {
                    route.Add(i);
                    i = 0;
                }
                else
                {
                    route.Add(i);
                    ++i;
                }
            }
        }
        /* VOCABULARY:
           START: Initial position of all pieces (-1). 
           Piece can exit start if player rolls a 6.

           HOMELANE: Colored lane that leads to home (the goal).
           Only pieces with matching color can enter homelane */
        private int startExit()
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
            // goal/home
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

        // Publics //////////////////////////////
        public int Position { get; private set; }
        public int Number { get; private set; }
        public string Color { get; private set; }
        public bool IsHome { get; private set; }
        // Privates /////////////////////////////
        private List<int> route;
    }
}
