using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AOTracker.Web.Services
{
    public class ServerDataBackgroundService : IHostedService, IDisposable
    {
        private ServerDataService serverDataService;
        private Timer timer;

        public ServerDataBackgroundService(ServerDataService serverDataService)
        {
            this.serverDataService = serverDataService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            serverDataService.UpdateServersData();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
