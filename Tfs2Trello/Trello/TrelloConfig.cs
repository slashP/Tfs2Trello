using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toml;

namespace Tfs2Trello.Trello
{
    public class TrelloConfig : ITrelloConfig
    {
        private static Dictionary<string, string> _usersDictionary;

        public long PollingInterval { get; private set; }
        public string Iteration { get; private set; }
        public string TfsProject { get; private set; }
        public IList<string> WorkItems { get; private set; }
        public string TfsUrl { get; private set; }
        public string TrelloKey { get; private set; }
        public string TrelloToken { get; private set; }
        public string BoardId { get; private set; }

        public void Initialize()
        {
            var location = System.Reflection.Assembly.GetAssembly(typeof (Program)).Location;
            var outputFolder = location.Split('\\').Reverse().Skip(1).Reverse().Aggregate((s, s1) => s + @"\" + s1);
            var config = File.ReadAllText(Path.Combine(outputFolder, "config.toml")).ParseAsToml();
            PollingInterval = config.PollingInterval;
            Iteration = config.Tfs.Iteration;
            TfsProject = config.Tfs.Project;
            WorkItems = ((object[]) config.Tfs.WorkItemTypes).Select(x => x.ToString()).ToList();
            TfsUrl = config.Tfs.Url;
            TrelloKey = config.Trello.Key;
            TrelloToken = config.Trello.Token;
            BoardId = config.Trello.BoardId;
            try {
                var userLists = (object[])config.Users;
                var tfsUsers = ((object[])userLists[0]).Select(x => x.ToString()).ToList();
                var trelloUsers = ((object[])userLists[1]).Select(x => x.ToString()).ToList();
                _usersDictionary = tfsUsers.Zip(trelloUsers, (tfs, trello) => new { trello, tfs }).ToDictionary(x => x.trello, x => x.tfs);
            }
            catch (NullReferenceException) {
                Console.WriteLine("Users are not correctly defined in the config file");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public string GetTrelloUsername(string name)
        {
            string user;
            _usersDictionary.TryGetValue(name, out user);
            return user;
        }
    }

    public interface ITrelloConfig {
        string GetTrelloUsername(string name);
        long PollingInterval { get; }
        string Iteration { get; }
        string TfsProject { get; }
        IList<string> WorkItems { get; }
        string TfsUrl { get; }
        string TrelloKey { get; }
        string TrelloToken { get; }
        string BoardId { get; }
        void Initialize();
    }
}
