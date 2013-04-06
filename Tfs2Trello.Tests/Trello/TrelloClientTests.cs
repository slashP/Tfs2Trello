using FakeItEasy;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tfs2Trello.Trello;
using TrelloNet;

namespace Tfs2Trello.Tests.Trello
{
    [TestClass]
    public class TrelloClientTests
    {
        private const string ListName = "my list";
        private ITrello _trelloFake;
        private TrelloClient _trelloClient;
        private const Color TaskColor = Color.Blue;

        [TestInitialize]
        public void Init()
        {
            _trelloFake = A.Fake<ITrello>();
            var unityContainer = new UnityContainer();
            Ioc.Configure(unityContainer);
            unityContainer.RegisterInstance(_trelloFake);
            var trelloLists = new []{new List{Name = ListName, Id = "1"}};
            A.CallTo(() => _trelloFake.Lists.ForBoard(TrelloClient.BoardId, ListFilter.All)).Returns(trelloLists);
            _trelloClient = new TrelloClient(A.Fake<ITrelloConfig>());
        }

        [TestCleanup]
        public void CleanUp()
        {
            _trelloFake = null;
            _trelloClient = null;
        }

        [TestMethod]
        public void DeleteAll_WhenOneCardAvailable_CardsDeleteCalledOnce()
        {
            var cards = new []{new Card()};
            A.CallTo(() => _trelloFake.Cards.ForBoard(A<IBoardId>.Ignored, BoardCardFilter.All)).Returns(cards);
            _trelloClient.DeleteAll();
            A.CallTo(() => _trelloFake.Cards.Delete(A<ICardId>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void DeleteAll_WhenTwoCardsAvailable_CardsDeleteCalledTwice()
        {
            var cards = new[] { new Card(), new Card() };
            A.CallTo(() => _trelloFake.Cards.ForBoard(A<IBoardId>.Ignored, BoardCardFilter.All)).Returns(cards);
            _trelloClient.DeleteAll();
            A.CallTo(() => _trelloFake.Cards.Delete(A<ICardId>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        #region Tests for adding cards / tasks

        [TestMethod]
        public void AddOrUpdateCard_WhenCardIsNew_CardsAddCalledOnce()
        {
            _trelloClient.AddOrUpdateCard(ListName, "my task", "my comment", "user", 1, TaskColor);
            A.CallTo(() => _trelloFake.Cards.Add("my task", A<IListId>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AddOrUpdateCard_WhenCardIsNew_ChangeNameNotCalled()
        {
            A.CallTo(() => _trelloFake.Cards.Add(A<string>.Ignored, A<IListId>.Ignored)).Returns(new Card { Name = "my task" });
            _trelloClient.AddOrUpdateCard(ListName, "my task", "my comment", "user", 1, TaskColor);
            A.CallTo(() => _trelloFake.Cards.ChangeName(A<ICardId>.Ignored, "my task")).MustNotHaveHappened();
        }

        [TestMethod]
        public void AddOrUpdateCard_WhenCardIsNew_CardMoveNotCalled()
        {
            _trelloClient.AddOrUpdateCard(ListName, "my task", "my comment", "user", 1, TaskColor);
            A.CallTo(() => _trelloFake.Cards.Move(A<ICardId>.Ignored, A<IListId>.Ignored)).MustNotHaveHappened();
        }

        #endregion
    }
}