using System;

namespace AOTracker.Web.Models
{
    public class ServerDataSnapshot
    { 
        public int ServerDataSnapshotId { get; set; }

        public string Name { get; set; }

        public string WebUrl { get; set; }

        public bool IsOnline { get; set; }

        public int TotalUsers { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
