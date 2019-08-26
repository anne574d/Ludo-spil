using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Ludo
{
    class Piece : Label
    {
        public Piece(string playerColor, int pieceNumber)
        {
            Color = playerColor;
            Number = pieceNumber;
            Position = -1;
            IsHome = false;
            createRoute();
        }

        public void SetParent(Form parent)
        {
            Parent = parent;
        }

        // /////////////////////////////////////////////////////////////////
        // Movement functions //////////////////////////////////////////////
        // /////////////////////////////////////////////////////////////////
        public void Move(int diceroll)
        {
            Position = LandsOnField(diceroll);
            if (Position == homeLaneEnd())
            {
                IsHome = true;
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

        public int LandsOnField(int diceroll)
        {
            // predicts where a piece will land given a diceroll
            int homeIndex = route.Count - 1;
            int routeIndex = route.IndexOf(Position) + diceroll;

            if (routeIndex >= route.Count)
            {
                // bounce back from home
                routeIndex = (2 * homeIndex) - routeIndex;
            }
            else if (CanLeaveStart(diceroll))
            {
                routeIndex = 0;
            }
            return route[routeIndex];
        }

        // /////////////////////////////////////////////////////////////////
        // Color specific positions ////////////////////////////////////////
        // /////////////////////////////////////////////////////////////////

        private void createRoute()
        {
            /* createRoute() is called in constructor.
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
                    ++i;
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

        public int IndexOnRoute(int i)
        {
            if (i < 0 || i >= route.Count)
            {
                return i;
            }
            else
            {
                return route.IndexOf(i);
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
