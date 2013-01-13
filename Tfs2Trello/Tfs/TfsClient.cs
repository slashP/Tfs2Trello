using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Tfs2Trello.Tfs
{
    public class TfsClient : ITfsClient
    {
        private DateTime _lastUpdate = DateTime.MinValue;

        public IEnumerable<TfsWorkItem> GetTfsWorkItemsToUpdate()
        {
            var workItemStore = GetWorkItemStore();
            var wiql = String.Format(@"
                           Select [State], [Title] 
                           From WorkItems
                           Where [Iteration Path] = '{0}'
                           And [Work Item Type] IN ({1})
                           And [Changed Date] > '{2}'
                           And [Team Project] = '{3}'", Iteration, WorkItemTypes, _lastUpdate, Project);
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
                           And [Team Project] = '{2}'", Iteration, WorkItemTypes, Project);
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

        private static string Iteration { get { return ConfigurationManager.AppSettings["Iteration"]; } }

        private static string Project { get { return ConfigurationManager.AppSettings["TfsProject"]; } }

        private static string WorkItemTypes 
        {
            get { return string.Join(", ", ConfigurationManager.AppSettings["WorkItemTypes"].Split(',').Select(x => "'" + x.Trim() + "'")); }
        }

        private static WorkItemStore GetWorkItemStore()
        {
            var tpc = new TfsTeamProjectCollection(new Uri(ConfigurationManager.AppSettings["TfsUrl"]));
            var workItemStore = tpc.GetService<WorkItemStore>();
            return workItemStore;
        }
    }
}