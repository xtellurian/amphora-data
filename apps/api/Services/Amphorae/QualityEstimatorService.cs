using System;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Amphorae
{
    public class QualityEstimatorService : IQualityEstimatorService
    {
        private readonly IAmphoraFileService fileService;
        private readonly ISignalService signalService;
        private readonly ILogger<QualityEstimatorService> logger;

        public QualityEstimatorService(IAmphoraFileService fileService,
                                       ISignalService signalService,
                                       ILogger<QualityEstimatorService> logger)
        {
            this.fileService = fileService;
            this.signalService = signalService;
            this.logger = logger;
        }

        public async Task<EnrichedDataQuality> GenerateDataQualitySummaryAsync(AmphoraModel amphora)
        {
            var summary = new EnrichedDataQuality();
            summary.Accuracy = amphora?.Quality?.Accuracy;
            summary.Completeness = amphora?.Quality?.Completeness;
            summary.Granularity = amphora?.Quality?.Granularity;
            summary.Reliability = amphora?.Quality?.Reliability;
            try
            {
                summary.CountSignals = amphora.V2Signals.Count;
                summary.CountFiles = (await fileService.Store.GetFilesAsync(amphora))?.Count ?? 0;
                if (summary.CountFiles > 0)
                {
                    summary.DaysSinceFilesLastUpdated = DaysSince(await fileService.Store.LastModifiedAsync(amphora));
                }

                if (summary.CountSignals > 0)
                {
                    summary.DaysSinceSignalsLastUpdated = DaysSince(new DateTime(await signalService.MaxWriteTimeAsync(amphora)));
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error when generating DataQualitySummary for amphora " + amphora.Id, ex);
            }

            return summary;
        }

        private int? DaysSince(DateTimeOffset? dt)
        {
            var ts = System.DateTime.Now - dt;
            if (ts.HasValue) { return ts.Value.Days; }
            else { return null; }
        }
    }
}