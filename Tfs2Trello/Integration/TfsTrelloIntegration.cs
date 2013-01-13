using System;
using System.Collections.Generic;
using System.Linq;
using Tfs2Trello.Tfs;
using Tfs2Trello.Trello;
using TrelloNet;

namespace Tfs2Trello.Integration
{
    public class TfsTrelloIntegration : ITfsTrelloIntegration
    {
        private readonly ITfsClient _tfsClient;
        private readonly ITrelloClient _trelloClient;

        public TfsTrelloIntegration(ITfsClient tfsClient, ITrelloClient trelloClient)
        {
            _tfsClient = tfsClient;
            _trelloClient = trelloClient;
        }

        public void Initialize()
        {
            _trelloClient.DeleteAll();
            var workItems = _tfsClient.GetAllWorkItems().ToList();
            UpdateWorkItems(workItems);
            _tfsClient.SetLastUpdate(DateTime.Now);
        }

        public void UpdateTrelloBoard()
        {
            var workItemsToChange = _tfsClient.GetTfsWorkItemsToUpdate().ToList();
            UpdateWorkItems(workItemsToChange);
            _tfsClient.SetLastUpdate(DateTime.Now);
        }

        private void UpdateWorkItems(IEnumerable<TfsWorkItem> workItemsToChange)
        {
            foreach (var tfsWorkItem in workItemsToChange) {
                var itemColor = GetWorkItemTypeColor(tfsWorkItem.WorkItemTypeName);
                _trelloClient.AddOrUpdateCard(tfsWorkItem.State, tfsWorkItem.Title, tfsWorkItem.Description, tfsWorkItem.AssignedTo, tfsWorkItem.Id, itemColor);
            }
        }

        private static Color GetWorkItemTypeColor(string workItemTypeName)
        {
            switch (workItemTypeName) {
                    case "Task":
                        return Color.Blue;
                    case "User Story":
                        return Color.Green;
                    case "Bug":
                        return Color.Red;
                default:
                    throw new NotSupportedException(string.Format("Work item type not supported ({0})", workItemTypeName));
            }
        }
    }
}