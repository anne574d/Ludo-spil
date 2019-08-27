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
    public class Game
    {
        public Game()
        {
            players = new List<Player>();
            gameOver = false;

            gui = new GUI(this);
            Application.Run(gui);
        }

        private void setupBoard()
        {
            board = new Field[76];
            for (int i = 0; i < board.Length; ++i)
            {
                board[i] = new Field(i, gui);
            }
        }

        public void AddPlayer(string color, bool isHuman)
        {
            players.Add(new Player(color, isHuman, gui));
        }

        public void DecidePlayOrder()
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

            players.Sort((p1, p2) => p1.SortOrder().CompareTo(p2.SortOrder()));

            while (players[0].Color != winnerColor)
            {
                // rotate play order
                Player temp = players[0];
                players.RemoveAt(0);
                players.Add(temp);
            }
            currentPlayer = 0;
        }

        public void StartGame()
        {
            setupBoard();
            gui.UpdateBoard(board);
        }

        private void NextPlayer()
        {
            currentPlayer++;
            if (currentPlayer >= players.Count)
            {
                currentPlayer = 0;
            }
        }

        public void NextTurn()
        {
            playerTurn(players[currentPlayer], 0);
        }

        private void playerTurn(Player player, int tries)
        {
            if (player.IsDone())
            {
                return;
            }

            gui.UpdateBoard(board);

            int roll = gui.GameDie.Value;
            validMoves = player.ValidMoves(roll);
            foreach (var i in validMoves)
            {
                if (i != -1)
                {
                    board[i].Highlight(true);
                }
            }

            if (validMoves.Count == 0)
            {
                Console.Write("No valid moves available. ");
                if (tries < 2)
                {
                    Console.Write($"{2 - tries} tries left. ");
                }
                else
                {
                    Console.Write("No tries left. ");
                    NextPlayer();
                }
            }
        }

        public void MovePiece(int fieldIndex)
        {
            if (validMoves.Contains(fieldIndex))
            {
                int roll = gui.GameDie.Value;
                Piece p;
                if (fieldIndex != -1)
                {
                    p = board[fieldIndex].OutgoingPiece();
                }
                else
                {
                    p = players[currentPlayer].GetPieceAtStart();
                }
                
                p.MovePiece(roll);
                board[p.Position].IncomingPiece(p);
            }
        }

        private void UpdateBoard()
        {
            for (int i = 0; i < board.Length; ++i)
            {
                board[i].Highlight(false); // reset from previous turn
            }
        }
            /*else
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
        }*/

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

                if (board[pos].IsHomeField())
                {
                    points = 10.0;
                    Debug.WriteLine($"Piece {piece} hits home ({points})");
                }
                else if (board[pos].EnemyDominated(ai.Color))
                {
                    points = 0.0 - howFarAhead;
                    Debug.WriteLine($"Piece {piece} will be send home ({points})");
                }
                else if (board[pos].SingleEnemy(ai.Color))
                {
                    points = 9 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} sends an enemy home ({points})");
                }
                else if (ai.GetPiece(piece).CanLeaveStart(diceroll))
                {
                    points = 7.0;
                    Debug.WriteLine($"Piece {piece} can exit start ({points})");
                }
                else if (!board[posStart].IsHomeLane() && board[pos].IsHomeLane())
                {
                    points = 8 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} enters home lane ({points})");
                }
                else if (board[pos].IsHomeLane())
                {
                    points = 4 + howFarAhead;
                    Debug.WriteLine($"Piece {piece} moves around on home lane ({points})");
                }
                else if (board[pos].HasFriendlyPiece(ai.Color))
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


        Field[] board;
        List<Player> players;
        bool gameOver;
        int currentPlayer;
        List<int> validMoves;

        GUI gui;
        PlayerSelectionMenu playerSelectMenu;

        static Random die = new Random();
    }
}
