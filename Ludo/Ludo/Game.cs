using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ludo
{
    class Game
    {
        public Game()
        {
            setupGame();
            selectPlayersMenu();
            decidePlayOrder();
            beginGame();
        }

        private void setupGame()
        {
            printer = new Printer();
            players = new List<Player>();
            die = new Random();
            gameover = false;

            board = new Field[76];
            for (int i = 0; i < board.Length; ++i)
            {
                board[i] = new Field();
            }
        }

        private void selectPlayersMenu()
        {
            int playerNumber;
            string input;
            List<string> availableColors = new List<string>() { "yellow", "blue", "red", "green" };

            do
            {
                Console.Write("Select number of players (2-4): ");
                input = Console.ReadLine();
            } while (!int.TryParse(input, out playerNumber) || playerNumber < 2 || playerNumber > 4);

            for (int i = 1; i <= playerNumber; ++i)
            {
                do
                {
                    Console.Write($"Player {i}, select your color ({string.Join(", ", availableColors)}): ");
                    input = Console.ReadLine();
                } while (!availableColors.Contains(input));

                availableColors.Remove(input);
                Player player = new Player(input);
                players.Add(player);
            }
        }

        private void decidePlayOrder()
        {
            // TODO
        }

        private void beginGame()
        {
            while (!gameover)
            {
                foreach (var player in players)
                {
                    printer.DrawBoard(board);
                    playerTurn(player);
                    if (player.IsDone())
                    {
                        printer.PrintEndScreen(player);
                        gameover = true;
                        break;
                    }
                }
            }
        }

        private void playerTurn(Player player)
        {
            Printer.ChangeFontColor(player.Color);
            Console.Write($"{Captitalize(player.Color)}'s turn. Press ENTER to roll die...");
            Console.ReadLine();
            int roll = rollDie();

            Console.Write($"{Captitalize(player.Color)} rolled {roll}. ");
            List<int> validMoves = player.MovablePieces(roll);

            if (validMoves.Count == 0)
            {
                Console.Write("No valid moves available. ");
                Console.ReadLine();
            }
            else
            {
                int selectedPiece;
                string input;
                do
                {
                    Console.Write($"Select a piece to move ({string.Join(", ", validMoves)}): ");
                    input = Console.ReadLine();
                } while (!int.TryParse(input, out selectedPiece) || !validMoves.Contains(selectedPiece));

                if (player.GetPiece(selectedPiece).Position != -1)
                {
                    // remove piece from old field
                    board[player.GetPiece(selectedPiece).Position].OutgoingPiece(player.GetPiece(selectedPiece));
                }
                // move piece
                player.GetPiece(selectedPiece).Move(roll);
                // update new field
                board[player.GetPiece(selectedPiece).Position].IncomingPiece(player.GetPiece(selectedPiece));
            }
        }

        static public string Captitalize(string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
        }

        private int rollDie()
        {
            return die.Next(1, 7);
        }

        Field[] board;
        Random die;
        List<Player> players;
        Printer printer;
        bool gameover;
    }
}
