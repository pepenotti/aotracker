using System;
using System.Collections.Generic;
using System.Linq;
using AOTracker.Web.Models;
using AOTracker.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AOTracker.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServersController : ControllerBase
    {

        private readonly ILogger<ServersController> logger;
        private readonly ServerDataService serverDataService;

        public ServersController(ILogger<ServersController> logger, ServerDataService serverDataService)
        {
            this.logger = logger;
            this.serverDataService = serverDataService;
        }

        [HttpGet]
        public IEnumerable<GetServersResponse> Get()
        {
            var snapshots = this.serverDataService.GetServersData();

            var servers = snapshots.Select(s => new
            {
                ServerName = s.ServerName,
                WebUrl = s.WebUrl
            }).Distinct()
            .Select(srv => new GetServersResponse
            {
                ServerName = srv.ServerName,
                WebUrl = srv.WebUrl
            }).ToList();

            foreach(var server in servers)
            {
                server.Snapshots = snapshots
                    .Where(snap => snap.ServerName == server.ServerName && snap.TimeStamp >= DateTime.Now.AddDays(-1))
                    .OrderBy(snap => snap.TimeStamp)
                    .ToList();
                server.IsOnline = server.Snapshots.Last().IsOnline;
                server.TotalUsers = server.Snapshots.Last().TotalUsers;
            }

            return servers;
        }
    }
}
