using System.Configuration;

namespace Tfs2Trello.Trello
{
    public static class Credentials
    {
        public static readonly string Key = ConfigurationManager.AppSettings["TrelloKey"];
        public static readonly string Token = ConfigurationManager.AppSettings["TrelloToken"];
    }
}
