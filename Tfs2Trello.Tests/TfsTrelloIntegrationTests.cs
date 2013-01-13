using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tfs2Trello.Integration;
using Tfs2Trello.Tfs;
using Tfs2Trello.Trello;
using TrelloNet;

namespace Tfs2Trello.Tests
{
    [TestClass]
    public class TfsTrelloIntegrationTests
    {
        private const string AssignedTo = "Me";
        private const string Description = "Description";
        private const string ListName = "Active";
        private const string MyTitle = "My title";
        private const int Id = 1;
        private const string Task = "Task";
        private const string UserStory = "User Story";
        private const string Bug = "Bug";
        private const Color UserStoryColor = Color.Green;
        private const Color BugColor = Color.Red;
        private const Color TaskColor = Color.Blue;
        private TfsTrelloIntegration _tfsTrelloIntegration;
        private ITfsClient _tfsClientFake;
        private ITrelloClient _trelloClientFake;

        [TestInitialize]
        public void Init()
        {
            _tfsClientFake = A.Fake<ITfsClient>();
            _trelloClientFake = A.Fake<ITrelloClient>();
            _tfsTrelloIntegration = new TfsTrelloIntegration(_tfsClientFake, _trelloClientFake);
        }

        [TestMethod]
        public void OnInitalizeAllCardsAreDeleted()
        {
            _tfsTrelloIntegration.Initialize();
            A.CallTo(() => _trelloClientFake.DeleteAll()).MustHaveHappened();
        }

        [TestMethod]
        public void OnInitializeAddOrUpdateTask()
        {
            SetupGetAllWorkItems(Task);
            _tfsTrelloIntegration.Initialize();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, TaskColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnInitializeAddOrUpdateUserStory()
        {
            SetupGetAllWorkItems(UserStory);
            _tfsTrelloIntegration.Initialize();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, UserStoryColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnInitializeAddOrUpdateBug()
        {
            SetupGetAllWorkItems(Bug);
            _tfsTrelloIntegration.Initialize();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, BugColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnInitializeAddOrUpdateBugAndAddOrUpdateUserStory()
        {
            var workItems = new[] {
                TfsWorkItem(Bug),
                TfsWorkItem(UserStory)
            };
            A.CallTo(() => _tfsClientFake.GetAllWorkItems()).Returns(workItems);
            _tfsTrelloIntegration.Initialize();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, BugColor)).MustHaveHappened();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, UserStoryColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnUpdateDoNotDeleteAll()
        {
            _tfsTrelloIntegration.UpdateTrelloBoard();
            A.CallTo(() => _trelloClientFake.DeleteAll()).MustNotHaveHappened();
        }

        [TestMethod]
        public void OnUpdateAddOrUpdateTaskIsCalled()
        {
            SetupGetTfsWorkItemsToUpdate(Task);
            _tfsTrelloIntegration.UpdateTrelloBoard();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, TaskColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnUpdateAddOrUpdateUserStory()
        {
            SetupGetTfsWorkItemsToUpdate(UserStory);
            _tfsTrelloIntegration.UpdateTrelloBoard();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, UserStoryColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnUpdateAddOrUpdateBug()
        {
            SetupGetTfsWorkItemsToUpdate(Bug);
            _tfsTrelloIntegration.UpdateTrelloBoard();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, BugColor)).MustHaveHappened();
        }

        [TestMethod]
        public void OnUpdateAddOrUpdateBugAndAddOrUpdateUserStory()
        {
            var workItems = new[] {
                TfsWorkItem(Bug),
                TfsWorkItem(UserStory)
            };
            A.CallTo(() => _tfsClientFake.GetTfsWorkItemsToUpdate()).Returns(workItems);
            _tfsTrelloIntegration.UpdateTrelloBoard();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, BugColor)).MustHaveHappened();
            A.CallTo(() => _trelloClientFake.AddOrUpdateCard(ListName, MyTitle, Description, AssignedTo, Id, UserStoryColor)).MustHaveHappened();
        }

        private void SetupGetAllWorkItems(string workItemTypeName)
        {
            var workItems = new[] {
                    TfsWorkItem(workItemTypeName)
            };
            A.CallTo(() => _tfsClientFake.GetAllWorkItems()).Returns(workItems);
        }

        private void SetupGetTfsWorkItemsToUpdate(string workItemTypeName)
        {
            var workItems = new[] {
                    TfsWorkItem(workItemTypeName)
            };
            A.CallTo(() => _tfsClientFake.GetTfsWorkItemsToUpdate()).Returns(workItems);
        }

        private static TfsWorkItem TfsWorkItem(string itemTypeName)
        {
            return new TfsWorkItem
                {
                    AssignedTo = AssignedTo,
                    Description = Description,
                    State = ListName,
                    Id = Id,
                    Title = MyTitle,
                    WorkItemTypeName = itemTypeName
                };
        }
    }
}
