using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Tfs2Trello.Trello;

namespace Tfs2Trello.Tfs
{
    public class TfsClient : ITfsClient
    {
        private readonly ITrelloConfig _trelloConfig;
        private DateTime _lastUpdate = DateTime.MinValue;

        public TfsClient(ITrelloConfig trelloConfig)
        {
            _trelloConfig = trelloConfig;
        }

        public IEnumerable<TfsWorkItem> GetTfsWorkItemsToUpdate()
        {
            var workItemStore = GetWorkItemStore();
            var wiql = String.Format(@"
                           Select [State], [Title] 
                           From WorkItems
                           Where [Iteration Path] = '{0}'
                           And [Work Item Type] IN ({1})
                           And [Changed Date] > '{2}'
                           And [Team Project] = '{3}'", _trelloConfig.Iteration, WorkItemTypes, _lastUpdate, _trelloConfig.TfsProject);
            var query = new Query(workItemStore, wiql, null, false);
            var cancellation = query.BeginQuery();
            return query.EndQuery(cancellation).Cast<WorkItem>().Select(ToTfsWorkItem);
        }

        public IEnumerable<TfsWorkItem> GetAllWorkItems()
        {
            var workItemStore = GetWorkItemStore();
            var wiql = String.Format(@"
                           Select [State], [Title] 
                           From WorkItems
                           Where [Iteration Path] = '{0}'
                           And [Work Item Type] IN ({1})
                           And [Team Project] = '{2}'", _trelloConfig.Iteration, WorkItemTypes, _trelloConfig.TfsProject);
            var workItemCollection = workItemStore.Query(wiql);
            return workItemCollection.Cast<WorkItem>().Select(ToTfsWorkItem);
        }

        public void SetLastUpdate(DateTime now)
        {
            _lastUpdate = now;
        }

        private static TfsWorkItem ToTfsWorkItem(WorkItem workItem)
        {
            return new TfsWorkItem {
                Description = workItem.Description,
                AssignedTo = workItem.Fields["Assigned To"].Value as string,
                State = workItem.State,
                Title = workItem.Title,
                WorkItemTypeName = workItem.Type.Name,
                Id = workItem.Id
            };
        }

        private string WorkItemTypes 
        {
            get { return string.Join(", ", _trelloConfig.WorkItems); }
        }

        private WorkItemStore GetWorkItemStore()
        {
            var tpc = new TfsTeamProjectCollection(new Uri(_trelloConfig.TfsUrl));
            var workItemStore = tpc.GetService<WorkItemStore>();
            return workItemStore;
        }
    }
}