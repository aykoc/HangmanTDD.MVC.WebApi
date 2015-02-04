using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HangmanTDD.Core;
using Newtonsoft.Json;

namespace HangmanTDD.Controllers
{
    public class GamesController : ApiController
    {
        private readonly HangmanFactory Factory = HangmanFactory.Current;
        
        public IEnumerable<string> Get()
        {
            var games = Factory.GetAllGames();
            return games.Select(JsonConvert.SerializeObject).ToArray();
        }

        public string Get(int id)
        {
            var game = Factory.GetGame(id);
            return JsonConvert.SerializeObject(game);
        }

        public void Post()
        {
            Factory.StartNewGame();
        }

        public void Post(int id, [FromBody]char value)
        {
            Factory.GetGame(id).MakeGuess(value);
        }
    }
}