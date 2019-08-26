using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Ludo
{
    class Game
    {
        public Game()
        {
            Players = new List<Player>();
            GameOver = false;

            Players.Add(new Player("red", true));
            Players.Add(new Player("blue", true));

            

            /*
            setupGame();
            startScreen();
            selectPlayersMenu();
            decidePlayOrder();
            beginGame(); */
        }

        public void SetupBoard(Form parent)
        {
            Board = new Field[76];
            for (int i = 0; i < Board.Length; ++i)
            {
                Board[i] = new Field(i, parent);
            }
        }

        private void startScreen()
        {
            // prints logo and gives option to start or quit
            ConsoleKeyInfo input;
            printer.PrintStartScreen();
            do
            {
                input = Console.ReadKey();
            } while (input.Key != ConsoleKey.Escape && input.Key != ConsoleKey.Enter);

            if (input.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
        }
        private void selectPlayersMenu()
        {
            List<string> availableColors = new List<string>() { "yellow", "blue", "red", "green" };

            // accept input of number of human players
            int humanPlayers;
            string input;
            Console.Write("Select number of players (1-4): ");
            do
            {
                input = Console.ReadLine();
            } while (!int.TryParse(input, out humanPlayers) || humanPlayers < 1 || humanPlayers > 4);

            // let human players pick their color
            for (int i = 1; i <= humanPlayers; ++i)
            {
                do
                {
                    Console.Write($"Player {i}, select your color ({string.Join(", ", availableColors)}): ");
                    input = Console.ReadLine().Trim().ToLower();
                } while (!availableColors.Contains(input));

                availableColors.Remove(input);
                Player player = new Player(input, true);
                Players.Add(player);
            }

            // add com players, whose color is one of the colors that's leftover
            if (humanPlayers < 4)
            {
                int comPlayers;
                int min = 0;
                int max = 4 - humanPlayers;
                if (humanPlayers == 1)
                {
                    min = 1;
                }

                Console.Write($"Select number of computer players ({min}-{max}): ");
                do
                {
                    input = Console.ReadLine();
                } while (!int.TryParse(input, out comPlayers) || comPlayers < min || comPlayers > max);

                for (int i = 0; i < comPlayers; ++i)
                {
                    Player comPlayer = new Player(availableColors[0], false);
                    Players.Add(comPlayer);
                    availableColors.Remove(availableColors[0]);
                }
            }
        }

        private void decidePlayOrder()
        {
            /* All players roll a die until one player rolls 
               more than the others. The winner starts and the 
               play order then goes clockwise around the board. */

            string winnerColor = "";
            int highestRoll;
            bool redo = true;
            while (redo)
            {
                highestRoll = 0;
                foreach (var player in Players)
                {
                    player.StartRoll = RollDie();
                    if (player.StartRoll > highestRoll)
                    {
                        highestRoll = player.StartRoll;
                        winnerColor = player.Color;
                        redo = false;
                    }
                    else if (player.StartRoll == highestRoll)
                    {
                        redo = true;
                    }
                }
            }

            // print result to users
            Console.WriteLine("\n\nEach player rolls a die to decide the play order: \n");
            foreach (var player in Players)
            {
                Console.WriteLine($"{Captitalize(player.Color)} rolled {player.StartRoll}. ");
            }
            Console.WriteLine($"\n{Captitalize(winnerColor)} rolled highest and will start. ");

            // sort players by yellow -> blue -> red -> green
            Players.Sort((p1, p2) => p1.SortOrder().CompareTo(p2.SortOrder()));

            while (Players[0].Color != winnerColor)
            {
                // rotate play order
                Player temp = Players[0];
                Players.RemoveAt(0);
                Players.Add(temp);
            }
            Console.ReadKey();
        }

        private void beginGame()
        {
            printer.PrintBoard();
            while (!GameOver)
            {
                foreach (var player in Players)
                {
                    playerTurn(player, 0);
                    if (player.IsDone())
                    {
                        printer.UpdateBoard(Players);
                        printer.PrintEndScreen(player);
                        GameOver = true;
                        break;
                    }
                }
            }
        }

        private void playerTurn(Player player, int tries)
        {
            if (player.IsDone())
            {
                return;
            }

            printer.UpdateBoard(Players);
            Printer.ChangeFontColor(player.Color);

            int roll = RollDie();
            Console.WriteLine($"{Captitalize(player.Color)}'s turn. {Captitalize(player.Color)} rolled [{roll}] ");

            List<int> validMoves = player.MovablePieces(roll);
            if (validMoves.Count == 0)
            {
                Console.Write("No valid moves available. ");
                if (tries < 2)
                {
                    Console.Write($"{2-tries} tries left. ");
                    Console.ReadKey();
                    playerTurn(player, tries + 1);
                }
                else
                {
                    Console.Write("No tries left. ");
                    Console.ReadKey();
                }
            }
            else
            {
                int selectedPiece;
                if (player.Human)
                {
                    Console.Write($"Select a piece to move ({string.Join(", ", validMoves)}): ");
                    string input;
                    do
                    {
                        input = Console.ReadKey().KeyChar.ToString();
                    } while (!int.TryParse(input, out selectedPiece) || !validMoves.Contains(selectedPiece));
                }
                else
                {
                    //selectedPiece = validMoves[RollDie(validMoves.Count)];
                    selectedPiece = aiDecision(player, validMoves, roll);
                    Console.Write($"{Captitalize(player.Color)} moves {selectedPiece}...");
                    Console.ReadKey();
                }

                if (player.GetPiece(selectedPiece).Position != -1)
                {
                    // remove piece from old field
                    Board[player.GetPiece(selectedPiece).Position].OutgoingPiece(player.GetPiece(selectedPiece));
                }
                // move piece
                player.GetPiece(selectedPiece).Move(roll);
                // update new field
                Board[player.GetPiece(selectedPiece).Position].IncomingPiece(player.GetPiece(selectedPiece));

                if (roll == 6)
                {
                    // Player gets an extra turn when they roll a 6
                    playerTurn(player, 0);
                }
            }
        }

        private int aiDecision(Player ai, List<int> validMoves, int diceroll)
        {
            Debug.WriteLine($"#### New decision ({ai.Color}) #### ");
            List<double> movesPoints = new List<double>();
            double points = 0;

            foreach (var piece in validMoves)
            {
                int posStart = ai.GetPiece(piece).Position;
                int pos = ai.GetPiece(piece).LandsOnField(diceroll);
                double howFarAhead = (double)ai.GetPiece(piece).IndexOnRoute(posStart) / 100;

                if (Board[pos].IsHomeField())
                {
                    points = 10.0;
                    Debug.WriteLine($"Piece {piece} hits home ({points})");
                }
                else if (Board[pos].EnemyDominated(ai.Color))
                {
                    points = 0.0 - howFarAhead;
                    Debug.WriteLine($"Piece {piece} will be send home ({points})");
                }
                else if (Board[pos].SingleEnemy(ai.Color))
                {
                    points = 9 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} sends an enemy home ({points})");
                }
                else if (ai.GetPiece(piece).CanLeaveStart(diceroll))
                {
                    points = 7.0;
                    Debug.WriteLine($"Piece {piece} can exit start ({points})");
                }
                else if (!Board[posStart].IsHomeLane() && Board[pos].IsHomeLane())
                {
                    points = 8 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} enters home lane ({points})");
                }
                else if (Board[pos].IsHomeLane())
                {
                    points = 4 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} moves around on home lane ({points})");
                }
                else if (Board[pos].HasFriendlyPiece(ai.Color))
                {
                    points = 6 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} can move to a friendly piece ({points})");
                }
                else // neutral move
                {
                    points = 5 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} will make a neutral move ({points})");
                }
                movesPoints.Add(points);
            }
            // returns validMove which corresponds to movePoints' max value. 
            Debug.WriteLine($" --> Piece {validMoves[movesPoints.IndexOf(movesPoints.Max())]} is the best move\n");

            return validMoves[movesPoints.IndexOf(movesPoints.Max())];
        }

        static public string Captitalize(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        static public int RollDie()
        {
            // roll 6-sided die
            return die.Next(1,7);
        }
        static public int RollDie(int n)
        {
            // roll n-sided die, returns 0 to (n-1)
            return die.Next(n);
        }
        public static Color GetColor(string color)
        {
            switch (color)
            {
                case "red": return Color.Red;
                case "blue": return Color.DeepSkyBlue;
                case "yellow": return Color.Gold;
                case "green": return Color.Green;

                default: return Color.Black;
            }
        }


        public Field[] Board;
        public List<Player> Players;
        public bool GameOver;

        Printer printer;

        static Random die = new Random();
    }
}
