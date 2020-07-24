using System;

namespace AOTracker.Web.Models
{
    public class ServerDataSnapshot : ServerData
    {
        public ServerDataSnapshot() { }

        public ServerDataSnapshot(ServerData sd)
        {
            this.ServerDataId = sd.ServerDataId;
            this.ServerName = sd.ServerName;
            this.WebUrl = sd.WebUrl;
            this.UsersEndpoint = sd.UsersEndpoint;
        }

        public bool IsOnline { get; set; }

        public int TotalUsers { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
