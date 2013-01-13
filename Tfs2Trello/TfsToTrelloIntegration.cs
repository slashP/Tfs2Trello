using System;
using System.Collections.Generic;
using System.Linq;
using Tfs2Trello.Tfs;
using Tfs2Trello.Trello;

namespace Tfs2Trello
{
    public class TfsToTrelloIntegration
    {
        private readonly TfsClient _tfsClient;
        private readonly TrelloClient _trelloClient;

        public TfsToTrelloIntegration()
        {
            _tfsClient = new TfsClient();
            _trelloClient = new TrelloClient();
        }

        public void Initialize()
        {
            _trelloClient.DeleteAll();
            var workItems = _tfsClient.GetAllWorkItems().ToList();
            UpdateWorkItems(workItems);
            _tfsClient.LastUpdate = DateTime.Now;
        }

        public void UpdateTrelloBoard()
        {
            var workItemsToChange = _tfsClient.GetTfsWorkItemsToUpdate().ToList();
            UpdateWorkItems(workItemsToChange);
            _tfsClient.LastUpdate = DateTime.Now;
        }

        private void UpdateWorkItems(IEnumerable<TfsWorkItem> workItemsToChange)
        {
            foreach (var tfsWorkItem in workItemsToChange) {
                switch (tfsWorkItem.WorkItemType.Name) {
                    case "Task":
                        _trelloClient.AddOrUpdateTask(tfsWorkItem.State, tfsWorkItem.Title, tfsWorkItem.Description, tfsWorkItem.AssignedTo, tfsWorkItem.Id);
                        break;
                    case "User Story":
                        _trelloClient.AddOrUpdateUserStory(tfsWorkItem.State, tfsWorkItem.Title, tfsWorkItem.Description, tfsWorkItem.AssignedTo, tfsWorkItem.Id);
                        break;
                    case "Bug":
                        _trelloClient.AddOrUpdateBug(tfsWorkItem.State, tfsWorkItem.Title, tfsWorkItem.Description, tfsWorkItem.AssignedTo, tfsWorkItem.Id);
                        break;
                }
            }
        }
    }
}