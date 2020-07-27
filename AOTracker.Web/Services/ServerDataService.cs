using AOTracker.Web.Data;
using AOTracker.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AOTracker.Web.Services
{
    public class ServerDataService
    {
        private List<ServerData> Data;
        private HttpClient HttpClient;
        private AOToolsContext context;

        public ServerDataService(string dataFilePath, AOToolsContext context)
        {
            this.Data = JsonConvert.DeserializeObject<List<ServerData>>(File.ReadAllText(dataFilePath));
            // [TODO => HttpClientFactory]
            this.HttpClient = new HttpClient();
            this.context = context;
            context.Database.EnsureCreated();
        }

        public List<ServerDataSnapshot> GetServersData() => this.context.ServerDataSnapshots.ToList();

        public async Task UpdateServersData() 
        {
            try
            {
                foreach (var server in this.Data)
                {
                    var stringResult = "";

                    var request = new HttpRequestMessage(HttpMethod.Get, server.UsersEndpoint);

                    using (var content = new StringContent("{'a' : 1}"))
                    {
                        request.Content = content;
                        request.Headers.Host = "aoxtreme.com.ar";
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var response = await this.HttpClient.SendAsync(request).ConfigureAwait(false);
                        stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }

                    var serverData = JsonConvert.DeserializeObject<UserEndpointResult>(stringResult);

                    var newSnapshot = new ServerDataSnapshot
                    {
                        ServerDataId = server.ServerDataId,
                        ServerName = server.ServerName,
                        WebUrl = server.WebUrl,
                        UsersEndpoint = server.UsersEndpoint,
                        IsOnline = serverData.Status.ToLower().Contains("online") && serverData.Players.HasValue,
                        TotalUsers = serverData.Players ?? 0,
                        TimeStamp = DateTime.UtcNow
                    };

                    this.context.ServerDataSnapshots.Add(newSnapshot);
                    this.context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
