using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using TrelloNet;

namespace Tfs2Trello.Trello
{
    public class TrelloClient : ITrelloClient
    {
        private readonly ITrelloConfig _trelloConfig;
        private readonly ITrello _trello;
        private static IDictionary<string, string> lists = new Dictionary<string, string>();
        private static List<TfsCard> cards;
        private static IDictionary<string, string> members = new Dictionary<string, string>();
        private static readonly string TrelloBoardId = ConfigurationManager.AppSettings["BoardId"];
        public static readonly BoardId BoardId = new BoardId(TrelloBoardId);

        public TrelloClient(ITrelloConfig trelloConfig)
        {
            _trelloConfig = trelloConfig;
            _trello = Ioc.Container.Resolve<ITrello>(new ParameterOverride("key", Credentials.Key));
            _trello.Authorize(Credentials.Token);
            lists = GetLists();
            members = GetMembers();
            cards = new List<TfsCard>();
        }

        public void AddOrUpdateCard(string listName, string name, string comment, string user, int id, Color workItemColor)
        {
            AddOrUpdateCard(listName, name, workItemColor, comment, user, id);
        }

        public void DeleteAll()
        {
            var board = _trello.Boards.WithId(TrelloBoardId);
            var allCards = _trello.Cards.ForBoard(board, BoardCardFilter.All);
            foreach (var card in allCards) {
                _trello.Cards.Delete(card);
            }
        }

        private void AddOrUpdateCard(string listName, string name, Color color, string comment, string user, int id)
        {
            var username = _trelloConfig.GetTrelloUsername(user);
            if (cards.Any(x => x.TfsId == id)) {
                UpdateTask(name, color, comment, username, id, listName);
            }
            else {
                AddTask(listName, name, id, username, color, comment);
            }
        }

        private void AddTask(string listName, string name, int id, string username, Color color, string comment)
        {
            var card = _trello.Cards.Add(name, GetListIdByName(listName));
            Console.WriteLine("Added work item: {0}", name);
            var tfsCard = card.ToTfsCard(id, listName);
            cards.Add(tfsCard);
            SetValues(listName, name, username, color, comment, tfsCard);
        }

        private void UpdateTask(string name, Color color, string comment, string user, int id, string listName)
        {
            var card = cards.First(x => x.TfsId == id);
            SetValues(listName, name, user, color, comment, card);
            Console.WriteLine("Updated work item: {0}", name);
        }

        private void SetValues(string listName, string name, string username, Color color, string comment, TfsCard tfsCard)
        {
            SetName(name, tfsCard);
            SetListName(listName, tfsCard);
            SetMember(username, tfsCard);
            SetLabel(color, tfsCard);
            SetComment(comment, tfsCard);
        }

        private void SetName(string name, TfsCard card)
        {
            if(card.Name == name) return;
            _trello.Cards.ChangeName(card, name);
            card.Name = name;
        }

        private void SetListName(string listName, TfsCard card)
        {
            if (card.ListName == listName) return;
            _trello.Cards.Move(card, GetListIdByName(listName));
            card.ListName = listName;
        }

        private void SetComment(string comment, TfsCard card)
        {
            if (string.IsNullOrEmpty(comment) || card.Desc == comment) return;
            _trello.Cards.AddComment(card, comment);
            card.Desc = comment;
        }

        private void SetLabel(Color color, TfsCard card)
        {
            if (card.LabelColor == color) return;
            _trello.Cards.AddLabel(card, color);
            if (card.LabelColor != null) _trello.Cards.RemoveLabel(card, (Color)card.LabelColor);
            card.LabelColor = color;
        }

        private void SetMember(string user, TfsCard card)
        {
            if (string.IsNullOrEmpty(user) || card.Username == user) return;
            if (!members.ContainsKey(user)) {
                Console.WriteLine("Users not defined correctly in config ({0})", user);
                Console.ReadKey();
                return;
            }
            var idOrUsername = members[user];
            _trello.Cards.AddMember(card, new MemberId(idOrUsername));
            card.Username = user;
        }


        private IDictionary<string, string> GetLists()
        {
            var forBoard = _trello.Lists.ForBoard(BoardId, ListFilter.All);
            return forBoard.ToDictionary(x => x.Name, x => x.Id);
        }

        private IDictionary<string, string> GetMembers()
        {
            return _trello.Members.ForBoard(BoardId).ToDictionary(x => x.Username, x => x.Id);
        }

        private static IListId GetListIdByName(string listName)
        {
            if (!lists.ContainsKey(listName)) {
                Console.WriteLine("The names of the lists does not match the possible states of work items. ({0})", listName);
                Console.ReadKey();
                Environment.Exit(0);
            }
            return new ListId(lists[listName]);
        }
    }

    public static class TfsCardExtension
    {
        public static TfsCard ToTfsCard(this Card card, int? id, string listname = null)
        {
            return new TfsCard {
                    TfsId = id,
                    ListName = listname,
                    Labels = card.Labels,
                    Name = card.Name,
                    Desc = card.Desc,
                    LabelColor = card.Labels != null && card.Labels.Any() ? card.Labels.First().Color : (Color?) null,
                    Id = card.Id,
                    IdList = card.IdList,
                    IdBoard = card.IdBoard,
                    IdShort = card.IdShort
                };
        }
    }

    public class TfsCard : Card
    {
        public int? TfsId { get; set; }
        public string ListName { get; set; }
        public Color? LabelColor { get; set; }
        public string Username { get; set; }
    }
}
