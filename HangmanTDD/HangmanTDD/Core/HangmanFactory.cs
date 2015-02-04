using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using HangmanTDD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HangmanTDD.Core
{
    public class HangmanFactory
    {
        #region Singletone Pattern
        private static readonly Lazy<HangmanFactory> instance = new Lazy<HangmanFactory>(() => new HangmanFactory());

        public static HangmanFactory Current
        {
            get
            {
                return instance.Value;
            }
        }
        #endregion

        private int NextGameID = 0;
        
        private readonly Random Randomizer = new Random();
        
        public List<string> WordList { get; private set; }
        
        private ConcurrentDictionary<int, HangmanGame> GameHash { get; set; } 

        protected HangmanFactory()
        {
            GameHash = new ConcurrentDictionary<int, HangmanGame>();
            try
            {
                var context = HttpContext.Current;
                var wordListFile = "Content/words.english";
                if (context != null)
                {
                    wordListFile = context.Server.MapPath("/Content/words.english");
                }
                WordList = File.ReadAllLines(wordListFile).ToList();
            }
            catch
            {
                throw new Exception("Wordlist can not be loaded.");
            }
        }

        public HangmanGame StartNewGame()
        {
            var game = new HangmanGame(Interlocked.Increment(ref NextGameID), WordList[Randomizer.Next(WordList.Count)]);
            GameHash.TryAdd(game.GameID, game);
            return game;
        }

        public HangmanGame GetGame(int id)
        {
            if (GameHash.ContainsKey(id))
            {
                return GameHash[id];
            }
            else
            {
                throw new ObjectNotFoundException(string.Format("Game not found with id={0}", id));
            }
        }

        public IEnumerable<HangmanGame> GetAllGames()
        {
            return GameHash.Values;
        }
    }
}