Usi
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSTV_Autopopulate.Models;


namespace DSTV_Autopopulate
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public IConfiguration Configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("The populator kicked off at exactly: {time}", DateTimeOffset.Now);
                string GTCNdataBaseConnector = Configuration["ConnectionStrings:GTCNDbConection"];
                int delay = int.Parse(Configuration["ValidData:delay"]); //Meant to be adjusted from the config poulator
                Populator pop = new Populator(_logger, Configuration);
                DSTVRequest APIRequest = new DSTVRequest
                {
                    serviceType = "dstv",
                    actionType = "cabletv_bundles"
                };
                var mboko = pop.HTTPClient(APIRequest);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
