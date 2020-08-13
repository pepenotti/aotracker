using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AOTracker.Web.Data;
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

        private readonly ServerDataService serverDataService;
        private AOToolsContext context;

        public ServersController(ILogger<ServersController> logger, ServerDataService serverDataService, AOToolsContext context)
        {
            this.serverDataService = serverDataService;
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<GetServersResponse> Get()
        {
            var snapshots = this.serverDataService.GetServersSnapshots();

            var servers = snapshots.Select(s => new
            {
                Name = s.Name,
                WebUrl = s.WebUrl
            }).Distinct()
            .Select(srv => new GetServersResponse
            {
                Name = srv.Name,
                WebUrl = srv.WebUrl
            }).ToList();

            if(snapshots.Any())
            {
                foreach (var server in servers)
                {
                    server.Snapshots = snapshots
                        .Where(snap => snap.Name == server.Name && snap.TimeStamp >= DateTime.UtcNow.AddHours(-6))
                        .OrderBy(snap => snap.TimeStamp)
                        .ToList();

                    if (server.Snapshots.Any())
                    {
                        server.IsOnline = server.Snapshots.Last().IsOnline;
                        server.TotalUsers = server.Snapshots.Last().TotalUsers;
                    }
                    else
                    {
                        server.IsOnline = false;
                        server.TotalUsers = 0;
                    }
                }
            }

            return servers;
        }

        [HttpPost]
        public async Task<PostServerResponse> AddServerAsync([FromBody] ServerData server)
        {
            var response = new PostServerResponse();

            var existingServer = context.ServersData.SingleOrDefault(sd =>
                sd.Name.Trim().ToLower() == server.Name.Trim().ToLower() ||
                sd.WebUrl.Trim().ToLower() == server.WebUrl.Trim().ToLower() ||
                sd.UsersEndpoint.Trim().ToLower() == server.UsersEndpoint.Trim().ToLower());

            if (existingServer != null)
            {
                var errorMessage = string.Empty;

                response.HasError = true;
                response.IsNameRepeated = existingServer.Name.Trim().ToLower() == server.Name.Trim().ToLower();
                response.IsUsersEndpointRepeated = existingServer.UsersEndpoint.Trim().ToLower() == server.UsersEndpoint.Trim().ToLower();
                response.IsWebRepeated = existingServer.WebUrl.Trim().ToLower() == server.WebUrl.Trim().ToLower();
            }

            response.WebIsNotValid = !await ValidURL(server.WebUrl);
            response.UsersEndpointIsNotValid = !await ValidURL(server.UsersEndpoint);


            if (response.WebIsNotValid || response.UsersEndpointIsNotValid)
                response.HasError = true;

            if (!response.HasError)
            {
                context.ServersData.Add(server);
                await context.SaveChangesAsync();
                response.ServerData = server;
            }

            return response;
        }

        private async Task<bool> ValidURL(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync(url);
                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
