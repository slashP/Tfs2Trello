using System;
using System.Collections.Generic;

namespace Tfs2Trello.Tfs
{
    public interface ITfsClient
    {
        IEnumerable<TfsWorkItem> GetTfsWorkItemsToUpdate();
        IEnumerable<TfsWorkItem> GetAllWorkItems();
        void SetLastUpdate(DateTime now);
    }
}