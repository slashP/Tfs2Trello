using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Tfs2Trello.Trello
{
    public class TrelloConfig : ITrelloConfig
    {
        private static Dictionary<string, string> _usersDictionary;

        static TrelloConfig()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var fileContent = File.ReadAllText(System.Reflection.Assembly.GetAssembly(typeof (Program)).Location + ".config");
            var configFile = XElement.Parse(fileContent);
            try {
                var users = configFile.Descendants("users").Descendants("user")
                        .Select(x => new { Name = x.Element("name").Value, TrelloUserName = x.Element("trelloUsername").Value });
                _usersDictionary = users.ToDictionary(x => x.Name, x => x.TrelloUserName);
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
    }
}
