using AOTracker.Web.Data;
using AOTracker.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AOTracker.Web.Services
{
    public class ServerDataService
    {
        private AOToolsContext context;
        private string dataFilePath;
        private bool initialized;

        public ServerDataService(string dataFilePath, AOToolsContext context)
        {
            this.context = context;
            context.Database.EnsureCreated();
            this.dataFilePath = dataFilePath;
            this.initialized = false;
        }

        public List<ServerDataSnapshot> GetServersSnapshots()
        {
            while (this.context.ChangeTracker.HasChanges()) { 

            }

            return this.context.ServerDataSnapshots.ToList();
        }

        public async Task TakeSnapshots()
        {
            UserEndpointResult serverData;

            foreach(var server in context.ServersData.ToList())
            {
                var httpClient = new HttpClient();
                var stringResult = String.Empty;

                var request = new HttpRequestMessage(HttpMethod.Get, server.UsersEndpoint);

                try
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var response = await httpClient.GetAsync(server.UsersEndpoint);
                    stringResult = await response.Content.ReadAsStringAsync();

                    stringResult = stringResult.Replace("onlines", "players");

                    serverData = JsonConvert.DeserializeObject<UserEndpointResult>(stringResult);
                }
                catch (Exception ex)
                {
                    serverData = new UserEndpointResult
                    {
                        Players = 0,
                        Status = "offline"
                    };
                }

                var newSnapshot = new ServerDataSnapshot
                {
                    Name = server.Name,
                    WebUrl = server.WebUrl,
                    IsOnline = (serverData.Players.HasValue && serverData.Players > 0) || (!string.IsNullOrWhiteSpace(serverData.Status) && serverData.Status.ToLower().Contains("online")),
                    TotalUsers = serverData.Players ?? 0,
                    TimeStamp = new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Utc)
                };

                this.context.ServerDataSnapshots.Add(newSnapshot);
                this.context.SaveChanges();
            }
        }

        public async Task InitializeServersData()
        {
            if (initialized)
                return;

            var serversData = JsonConvert.DeserializeObject<List<ServerData>>(File.ReadAllText(this.dataFilePath));

            foreach(var serverData in serversData)
            {
                try
                {
                    var server = context.ServersData.SingleOrDefault(sd => sd.Name == serverData.Name);

                    if(server == null)
                    {
                        context.ServersData.Add(serverData);
                        await context.SaveChangesAsync();
                    }

                }
                catch (Exception) { }
            }

            initialized = true;
        }
    }
}
