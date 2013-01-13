using TrelloNet;

namespace Tfs2Trello.Trello
{
    public interface ITrelloClient {
        void AddOrUpdateCard(string listName, string name, string comment, string user, int id, Color workItemColor);
        void DeleteAll();
    }
}