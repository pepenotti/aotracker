using System.Collections.Generic;

namespace AOTracker.Web.Models
{
    public class GetServersResponse
    {
        public string Name { get; set; }

        public string WebUrl { get; set; }

        public int TotalUsers { get; set; }

        public bool IsOnline { get; set; }

        public List<ServerDataSnapshot> Snapshots { get; set; }
    }
}