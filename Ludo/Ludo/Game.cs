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
        const int AIDELAY = 700;

        public Game()
        {
            validMoves = new List<Piece>();
            players = new List<Player>();
            gameOver = false;
            tries = 0;

            gui = new GUI(this);
            Application.Run(gui);
        }

        public void Reset()
        {
            /*
            validMoves = new List<int>();
            players = new List<Player>();
            gameOver = false;
            tries = 0;

            gui = new GUI(this);
            Application.Restart();
            */
        }


        // Board drawing ///////////////////////////////////////////////////////
        private void setupBoard()
        {
            // draw fields on board
            board = new Field[76];
            for (int i = 0; i < board.Length; ++i)
            {
                board[i] = new Field(i, gui);
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

        // Game/player setup ///////////////////////////////////////////////
        public void AddPlayer(string color, bool isHuman)
        {
            players.Add(new Player(color, isHuman, gui));
        }

        private void decidePlayOrder()
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
            decidePlayOrder();
            setupBoard();
            NextPlayer();
        }

        // Gameplay //////////////////////////////////////////////////////////////////
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
            if (gameOver)
            {
                return;
            }

            if (!players[currentPlayer].Human)
            {
                // have COM player click die
                Timer timer = new Timer { Enabled = true, Interval = AIDELAY };
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
            if (gameOver)
            {
                return;
            }

            if (!players[currentPlayer].Human)
            {
                Timer timer = new Timer { Enabled = true, Interval = AIDELAY };
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    // click piece with best movement score to move it
                    aiDecision().OnClick(null, null);
                };
            }
            else
            {
                // wait for user to click field/start
                gui.Dialog.WriteLine("Select a piece to move...");
            }
        }

        private void checkIfDone(Player player)
        {
            if (player.IsDone())
            {
                gameOver = true;
                gui.ShowEndScreen(player);
            }
        }

        private void playerTurn()
        {
            if (gameOver)
            {
                return;
            }

            int roll = gui.GameDie.Value;
            validMoves = players[currentPlayer].MoveablePieces(roll);

            if (validMoves.Count > 0)
            {
                awaitPieceMove();
            }
            else
            {
                gui.GameDie.Enable();
                gui.Dialog.WriteLine($"No valid moves available. ");
                if (tries < 2)
                {
                    ++tries;
                    gui.Dialog.WriteLine($"{3 - tries} tries left. ");
                    awaitDiceroll();                    
                }
                else
                {
                    gui.Dialog.WriteLine("No tries left. ");
                    NextPlayer();
                }
            }
        }

        // Input, called from GUI/AI ////////////////////////////////////////
        public void DieRolled()
        {
            playerTurn();
        }

        public void StartClicked(string color)
        {
            if (color == players[currentPlayer].Color)
            {
                MovePiece(players[currentPlayer].GetPieceAtStart());
            }
        }

        public void FieldClicked(int index)
        {
            MovePiece(board[index].GetPiece());
        }

        public void MovePiece(Piece piece)
        {
            if (validMoves.Contains(piece))
            {
                // move piece
                int roll = gui.GameDie.Value;
                if (!piece.IsAtStart())
                {
                    board[piece.Position].OutgoingPiece(piece);
                }
                piece.MovePiece(roll);
                board[piece.Position].IncomingPiece(piece);

                // prepare for next turn
                gui.GameDie.Enable();
                resetBoard();
                validMoves.Clear();
                checkIfDone(players[currentPlayer]);

                // check for extra turn
                if (roll == 6)
                {
                    gui.Dialog.WriteLine($"{Captitalize(players[currentPlayer].Color)} rolled a 6 and gets another turn. ");
                    awaitDiceroll();
                }
                else
                {
                    NextPlayer();
                }                
            }
            else
            {
                //gui.Dialog.WriteLine("Invalid move. Click on one of the highlighted fields. ");
            }
        }

        // AI decision makin ////////////////////////////////////////////////////////////
        private Piece aiDecision()
        {
            List<double> movesPoints = new List<double>();
            double points = 0;

            Player ai = players[currentPlayer]; // redudent, makes code nicer looking
            int roll = gui.GameDie.Value;

            foreach (Piece piece in validMoves)
            {
                int posStart = piece.Position;
                int posEnd = piece.LandsOnField(roll);
                double howFarAhead = (double)piece.IndexOnRoute(posStart) / 100;

                if (board[posEnd].IsHomeField())
                {
                    // will hit home
                    points = 10.0;
                }
                else if (board[posEnd].EnemyDominated(ai.Color))
                {
                    // will be send home
                    points = 0.0 - howFarAhead;
                }
                else if (board[posEnd].SingleEnemy(ai.Color))
                {
                    // will send an enemy home
                    points = 9 + howFarAhead;
                }
                else if (piece.CanLeaveStart(roll))
                {
                    // will exit start
                    points = 7.0;
                }
                else if (!board[posStart].IsHomeLane() && board[posEnd].IsHomeLane())
                {
                    // will enter home lane
                    points = 8 + howFarAhead;
                }
                else if (board[posEnd].IsHomeLane())
                {
                    // will move around on home lane
                    points = 4 + howFarAhead;
                }
                else if (board[posEnd].HasFriendlyPiece(ai.Color))
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
            // return number of piece with best move
            return validMoves[movesPoints.IndexOf(movesPoints.Max())];
        }

        // Statics ///////////////////////////////////////////////////////////////
        static public string Captitalize(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        // Field //////////////////////////////////////////////////////////////////
        Field[] board;
        List<Player> players;
        int currentPlayer;
        List<Piece> validMoves;
        int tries;
        bool gameOver;

        GUI gui;
    }
}
