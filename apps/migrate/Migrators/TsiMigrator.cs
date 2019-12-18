using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Signals;
using Amphora.Common.Services.Azure;
using Amphora.Migrate.Contracts;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Migrate.Migrators
{
    public class TsiMigrator : IMigrator
    {
        private readonly ITsiService tsiService;
        private readonly ILogger<TsiMigrator> logger;
        private readonly EventHubSender eventHubSender;

        public TsiMigrator(ITsiService tsiService,
                           ILogger<TsiMigrator> logger,
                           EventHubSender eventHubSender)
        {
            this.tsiService = tsiService;
            this.logger = logger;
            this.eventHubSender = eventHubSender;
        }

        public async Task MigrateAsync()
        {
            //signalService.RunGetEventsAsync()
            var instances = await tsiService.GetInstancesAsync();

            var start = System.DateTime.UtcNow.AddYears(-1);
            var end = System.DateTime.UtcNow.AddMonths(2);
            var range = new DateTimeRange(start, end);


            foreach (var instance in instances)
            {
                var events = await GetEventsAsync(instance, range);
                var payload = ConstructPayload(events);
                await eventHubSender.SendToEventHubAsync(payload);
            }
        }

        private IList<Dictionary<string, object?>> ConstructPayload(QueryResultPage page)
        {
            var events = page.Timestamps.Select(_ => new Dictionary<string, object?> { { SpecialProperties.Timestamp, _ } }).ToList();
            for(var i = 0; i < page.Timestamps.Count; i++)
            {
                var currentEvent = events[i];     
                foreach (var prop in page.Properties)
                {
                    currentEvent[prop.Name] = prop.Values[i];
                }
            }
            return events;
        }

        private async Task<QueryResultPage> GetEventsAsync(TimeSeriesInstance instance, DateTimeRange range)
        {
            string? continuationToken = null;
            var timestamps = new List<DateTime?>();
            var properties = new List<PropertyValues>();
            QueryResultPage page;
            try
            {
                do
                {
                    page = await tsiService.RunGetEventsAsync(instance.TimeSeriesId, range);
                    continuationToken = page.ContinuationToken;
                    properties.AddRange(page.Properties);
                    timestamps.AddRange(page.Timestamps);
                }
                while (continuationToken != null);
            }
            catch (TsiErrorException tsiEx)
            {
                logger.LogError(tsiEx.Message);
            }

            logger.LogInformation($"Loaded {properties.Count} properties");
            if (properties.Count != timestamps.Count)
            {
                logger.LogInformation($"There were {properties.Count} properties and {timestamps.Count} timestamps");
            }
            return new QueryResultPage(null, timestamps, properties);
        }


    }
}