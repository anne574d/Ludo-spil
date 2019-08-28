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
            validMoves = new List<int>();
            players = new List<Player>();
            gameOver = false;
            tries = 0;

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

            foreach (var color in new List<string>() { "yellow", "blue", "red", "green" })
            {
                gui.DrawStartZone(color);
            }

        }

        public void AddPlayer(string color, bool isHuman)
        {
            players.Add(new Player(color, isHuman, gui));
        }

        public void DecidePlayOrder()
        {
            Random rng = new Random();
            int rnd = rng.Next(8);

            // sort play order to be yellow -> blue -> red -> green (clockwise around board)
            players.Sort((p1, p2) => p1.SortOrder().CompareTo(p2.SortOrder()));

            // rotate play order random number of times
            for(int i = 0; i < rnd; ++i)
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
            NextPlayer();
        }

        private void NextPlayer()
        {
            tries = 0;

            currentPlayer++;
            if (currentPlayer >= players.Count)
            {
                currentPlayer = 0;
            }

            //gui.Dialog.ChangeColor(players[currentPlayer].Color);
            gui.ChangeBorderColor(players[currentPlayer].Color);
            gui.Dialog.WriteLine($"{Captitalize(players[currentPlayer].Color)}'s turn. ");

            awaitDiceroll();
        }

        private void awaitDiceroll()
        {
            if (!players[currentPlayer].Human)
            {
                // have COM player click die
                Timer timer = new Timer { Enabled = true, Interval = 700 };
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    gui.GameDie.OnClick(null, null);
                };
            }
            else
            {
                // wait for user to click die
                gui.Dialog.WriteLine("Click the die to roll it. ");
            }
        }
        private void awaitPieceMove()
        {
            if (!players[currentPlayer].Human)
            {
                Timer timer = new Timer { Enabled = true, Interval = 700 };
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    int bestMove = aiDecision();
                    if (bestMove == -1)
                    {
                        StartClicked(players[currentPlayer].Color);
                    }
                    else
                    {
                        board[aiDecision()].OnClick(null, null);
                    }
                };
            }
            else
            {
                gui.Dialog.WriteLine("Select a piece to move...");
                // wait for user to click field/start
            }
        }

        public void DieRolled()
        {
            playerTurn(players[currentPlayer]);
        }

        private void playerTurn(Player player)
        {
            if (player.IsDone())
            {
                gameOver = true;
                return;
            }

            int roll = gui.GameDie.Value;
            validMoves = player.ValidMoves(roll);

            foreach (var i in validMoves)
            {
                if (i != -1)
                {
                    board[i].Highlight(true);
                }
            }

            if (validMoves.Count > 0)
            {
                awaitPieceMove();
            }
            else
            {
                gui.GameDie.Enable();
                if (tries < 2)
                {
                    ++tries;
                    gui.Dialog.WriteLine($"No valid moves available. {3 - tries} tries left. ");
                    awaitDiceroll();                    
                }
                else
                {
                    gui.Dialog.WriteLine("No valid moves available. No tries left. ");
                    NextPlayer();
                }
            }
        }

        public void StartClicked(string color)
        {
            if (color == players[currentPlayer].Color)
            {
                MovePiece(-1);
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

                gui.GameDie.Enable();
                resetBoard(); 

                if (roll == 6)
                {
                    gui.Dialog.WriteLine($"{Captitalize(players[currentPlayer].Color)} rolled a 6 and gets another turn. ");
                    awaitDiceroll();
                }
                else
                {
                    NextPlayer();
                }
                validMoves.Clear();
            }
            else
            {
                //gui.Dialog.WriteLine("Invalid move. Click on one of the highlighted fields. ");
            }
        }

        private void resetBoard()
        {
            for (int i = 0; i < board.Length; ++i)
            {
                // remove valid moves highlight
                board[i].Highlight(false); 
            }
        }

        private int aiDecision()
        {
            List<double> movesPoints = new List<double>();
            double points = 0;

            Player ai = players[currentPlayer]; // redudent, makes code nicer looking
            int roll = gui.GameDie.Value;
            int piece;

            foreach (var fieldIndex in validMoves)
            {
                if (fieldIndex == -1)
                {
                    piece = ai.GetPieceAtStart().Number;
                }
                else
                {
                    piece = board[fieldIndex].GetPiece().Number;
                }
               
                int posStart = ai.GetPiece(piece).Position;
                int pos = ai.GetPiece(piece).LandsOnField(roll);
                double howFarAhead = (double)ai.GetPiece(piece).IndexOnRoute(posStart) / 100;

                if (board[pos].IsHomeField())
                {
                    // will hit home
                    points = 10.0;
                }
                else if (board[pos].EnemyDominated(ai.Color))
                {
                    // will be send home
                    points = 0.0 - howFarAhead;
                }
                else if (board[pos].SingleEnemy(ai.Color))
                {
                    // will send an enemy home
                    points = 9 + howFarAhead;
                }
                else if (ai.GetPiece(piece).CanLeaveStart(roll))
                {
                    // will exit start
                    points = 7.0;
                }
                else if (!board[posStart].IsHomeLane() && board[pos].IsHomeLane())
                {
                    // will enter home lane
                    points = 8 + howFarAhead;
                }
                else if (board[pos].IsHomeLane())
                {
                    // will move around on home lane
                    points = 4 + howFarAhead;
                }
                else if (board[pos].HasFriendlyPiece(ai.Color))
                {
                    // will group up with friendly piece
                    points = 6 + howFarAhead;
                }
                else
                {
                    // neutral move
                    points = 5 + howFarAhead;
                }
                movesPoints.Add(points);
            }
            return validMoves[movesPoints.IndexOf(movesPoints.Max())];
        }

        static public string Captitalize(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        Field[] board;
        List<Player> players;
        int currentPlayer;

        bool gameOver;

        List<int> validMoves;
        int tries; 

        GUI gui;
    }
}
