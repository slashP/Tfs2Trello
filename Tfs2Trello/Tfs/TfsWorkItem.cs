﻿
namespace Tfs2Trello.Tfs
{
    public class TfsWorkItem
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string WorkItemTypeName { get; set; }
        public string Description { get; set; }
    }
}