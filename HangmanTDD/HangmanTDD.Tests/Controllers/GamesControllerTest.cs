using System;
using System.Collections.Generic;
using System.Linq;
using HangmanTDD.Controllers;
using HangmanTDD.Core;
using HangmanTDD.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HangmanTDD.Tests.Controllers
{
    [TestClass]
    public class GamesControllerTest
    {
        [TestMethod]
        public void PlayGame()
        {
            var controller = new GamesController();

            var result = controller.Get();
            var list = result.Select(JsonConvert.DeserializeObject<HangmanGame>).ToArray();
            Assert.IsTrue(!list.Any());

            controller.Post();

            result = controller.Get();
            list = result.Select(JsonConvert.DeserializeObject<HangmanGame>).ToArray();
            Assert.AreEqual(list.Count(), 1);

            var id = list.First().GameID;

            var firstGame = JsonConvert.DeserializeObject<HangmanGame>(controller.Get(id));
            
            Assert.IsNull(firstGame.GetWord()); //dont hint the word to the user.

            Assert.AreEqual(firstGame.Status, GameStatus.Busy);
            Assert.AreEqual(firstGame.TriesLeft, 11);
            Assert.IsFalse(firstGame.Guess.Any(x => x != '.'));
            
            firstGame = JsonConvert.DeserializeObject<HangmanGame>(controller.Get(id));
            
            var nextChar = 'a';
            var triesLeft = 11;
            var guess = firstGame.Guess;
            do
            {
                controller.Post(id, nextChar);
                firstGame = JsonConvert.DeserializeObject<HangmanGame>(controller.Get(id));

                if (firstGame.Status == GameStatus.Success)
                {
                    Assert.IsFalse(firstGame.Guess.Any(x => x == '.'));
                }

                if (firstGame.TriesLeft == triesLeft)
                {
                    Assert.AreNotEqual(guess, firstGame.Guess);
                    Assert.IsTrue(firstGame.Guess.Contains(nextChar));
                }
                else
                {
                    Assert.AreEqual(guess, firstGame.Guess);
                    --triesLeft;
                }

                if (firstGame.TriesLeft == 0)
                {
                    Assert.AreEqual(firstGame.Status, GameStatus.Fail);
                }
                guess = firstGame.Guess;
                ++nextChar;
            } while (firstGame.TriesLeft > 0 && firstGame.Status == GameStatus.Busy);


            controller.Post();
            
            result = controller.Get();
            list = result.Select(JsonConvert.DeserializeObject<HangmanGame>).ToArray();
            Assert.AreEqual(list.Count(), 2);
            id = list.Last().GameID;

            var secondGame = JsonConvert.DeserializeObject<HangmanGame>(controller.Get(id));
            var clrGame = HangmanFactory.Current.GetGame(id);

            var answer = clrGame.GetWord();
            Assert.AreEqual(answer.Length, secondGame.Guess.Length);

            foreach (var a in answer)
            {
                if(secondGame.Guess.Contains(a))
                    continue;

                controller.Post(id, a);
                secondGame = JsonConvert.DeserializeObject<HangmanGame>(controller.Get(id));
                Assert.AreEqual(secondGame.TriesLeft, 11);

                if(secondGame.Status == GameStatus.Success)
                    break;
            }

            Assert.AreEqual(secondGame.Status, GameStatus.Success);
            Assert.AreEqual(secondGame.TriesLeft, 11);
            Assert.AreEqual(secondGame.Guess, answer);

        }
    }
}
