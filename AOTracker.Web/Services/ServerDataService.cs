using AOTracker.Web.Data;
using AOTracker.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
            this.HttpClient = new HttpClient();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.HttpClient.DefaultRequestHeaders.Host = "aoxtreme.com.ar";
            this.context = context;
        }

        public List<ServerDataSnapshot> GetServersData() => this.context.ServerDataSnapshots.ToList();

        public async Task UpdateServersData() 
        {
            try
            {
                foreach (var server in this.Data)
                {
                    var stringResult = "";

                    // [TODO => HttpClientFactory]
                    var client = new HttpClient(); //You should extract this and reuse the same instance multiple times.
                    var request = new HttpRequestMessage(HttpMethod.Get, server.UsersEndpoint);

                    using (var content = new StringContent("{'a' : 1}"))
                    {
                        request.Content = content;
                        request.Headers.Host = "aoxtreme.com.ar";
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var response = await client.SendAsync(request).ConfigureAwait(false);
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
                        TimeStamp = DateTime.Now
                    };

                    this.context.ServerDataSnapshots.Add(newSnapshot);
                    await this.context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
