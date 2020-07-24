using AOTracker.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AOTracker.Web.Data
{
    public class AOToolsContext : DbContext
    {
        public AOToolsContext(DbContextOptions options) : base(options) { }

        public DbSet<ServerData> ServersData { get; set; }

        public DbSet<ServerDataSnapshot> ServerDataSnapshots { get; set; }
    }
}