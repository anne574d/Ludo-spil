using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace Ludo
{
    class Game
    {
        public Game()
        {
            setupGame();
            startScreen();
            selectPlayersMenu();
            decidePlayOrder();
            beginGame();
        }

        private void setupGame()
        {
            printer = new Printer();
            players = new List<Player>();
            gameover = false;

            board = new Field[76];
            for (int i = 0; i < board.Length; ++i)
            {
                board[i] = new Field(i);
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
                players.Add(player);
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
                    players.Add(comPlayer);
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
                foreach (var player in players)
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
            foreach (var player in players)
            {
                Console.WriteLine($"{Captitalize(player.Color)} rolled {player.StartRoll}. ");
            }
            Console.WriteLine($"\n{Captitalize(winnerColor)} rolled highest and will start. ");

            // sort players by yellow -> blue -> red -> green
            players.Sort((p1, p2) => p1.SortOrder().CompareTo(p2.SortOrder()));

            while (players[0].Color != winnerColor)
            {
                // rotate play order
                Player temp = players[0];
                players.RemoveAt(0);
                players.Add(temp);
            }
            Console.ReadKey();
        }

        private void beginGame()
        {
            printer.PrintBoard();
            while (!gameover)
            {
                foreach (var player in players)
                {
                    playerTurn(player, 0);
                    if (player.IsDone())
                    {
                        printer.UpdateBoard(players);
                        printer.PrintEndScreen(player);
                        gameover = true;
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

            printer.UpdateBoard(players);
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
                    board[player.GetPiece(selectedPiece).Position].OutgoingPiece(player.GetPiece(selectedPiece));
                }
                // move piece
                player.GetPiece(selectedPiece).Move(roll);
                // update new field
                board[player.GetPiece(selectedPiece).Position].IncomingPiece(player.GetPiece(selectedPiece));

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
            List<int> movesPoints = new List<int>();

            foreach (var piece in validMoves)
            {
                int posStart = ai.GetPiece(piece).Position;
                int pos = ai.GetPiece(piece).LandsOnField(diceroll);

                if (board[pos].IsHomeField())
                {
                    Debug.WriteLine($"Piece {piece} hits home");
                    movesPoints.Add(10);
                }
                else if (board[pos].EnemyDominated(ai.Color))
                {
                    Debug.WriteLine($"Piece {piece} will be send home");
                    movesPoints.Add(0);
                }
                else if (board[pos].SingleEnemy(ai.Color))
                {
                    Debug.WriteLine($"Piece {piece} sends an enemy home");
                    movesPoints.Add(9);
                }
                else if (ai.GetPiece(piece).CanLeaveStart(diceroll))
                {
                    Debug.WriteLine($"Piece {piece} can exit start");
                    movesPoints.Add(7);
                }
                else if (!board[posStart].IsHomeLane() && board[pos].IsHomeLane())
                {
                    Debug.WriteLine($"Piece {piece} enters home lane");
                    movesPoints.Add(8);
                }
                else if (board[pos].HasFriendlyPiece(ai.Color))
                {
                    Debug.WriteLine($"Piece {piece} can move to a friendly piece");
                    movesPoints.Add(6);
                }
                else if (board[pos].IsHomeLane())
                {
                    Debug.WriteLine($"Piece {piece} moves around on home lane");
                    movesPoints.Add(4);
                }
                else // neutral move
                {
                    Debug.WriteLine($"Piece {piece} will make a neutral move");
                    movesPoints.Add(5);
                }
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

        Field[] board;
        List<Player> players;
        Printer printer;
        bool gameover;

        static Random die = new Random();
    }
}
