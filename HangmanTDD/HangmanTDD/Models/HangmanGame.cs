using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangmanTDD.Models
{

    public enum GameStatus
    {
        Busy,
        Fail,
        Success
    }

    public class HangmanGame
    {
        public int GameID { get; set; }

        public string Guess { get; set; }

        public int TriesLeft { get; set; }

        public GameStatus Status { get; set; }
        
        private string Word { get; set; }

        public HangmanGame(int id, string word)
        {
            GameID = id;
            Word = word;
            TriesLeft = 11;
            Status = GameStatus.Busy;
            if (word != null)
            {
                Guess = string.Join(string.Empty, word.Select(x => ".").ToArray());
            }
        }

        public void MakeGuess(char @char)
        {
            if (Status == GameStatus.Fail)
                throw new Exception("You have no tries left.");
            if (Status == GameStatus.Success)
                throw new Exception("You win this game. Start a new game.");
            @char = char.ToLowerInvariant(@char);
            if (@char < 'a' || @char > 'z')
                throw new Exception("You can use only character set [a-z].");

            var guess = string.Concat(Word.Select((x, y) => x == @char ? x : Guess[y]));

            if (guess == Guess) //Guessing a correct letter doesn’t decrement the amount of tries left
                --TriesLeft;

            Guess = guess;

            if (TriesLeft == 0)
                Status = GameStatus.Fail;
            else if (Word == Guess)
                Status = GameStatus.Success;
        }

        public string GetWord()
        {
            return Word;
        }
    }
}