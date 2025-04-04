using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Meritocious.Common.Enums;
using Meritocious.AI.MeritScoring.Interfaces;

namespace Meritocious.AI.Search
{
    public class VectorIndexMaintenanceService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<VectorIndexMaintenanceService> logger;
        private readonly ConcurrentQueue<IndexingJob> indexingQueue;
        private readonly SemaphoreSlim indexingSemaphore;
        private readonly TimeSpan batchDelay = TimeSpan.FromSeconds(5);
        private readonly int maxBatchSize = 100;
        private readonly int maxConcurrentIndexing = 3;

        public VectorIndexMaintenanceService(
            IServiceScopeFactory scopeFactory,
            ILogger<VectorIndexMaintenanceService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            indexingQueue = new ConcurrentQueue<IndexingJob>();
            indexingSemaphore = new SemaphoreSlim(maxConcurrentIndexing);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Initialize vector collections
                using (var scope = scopeFactory.CreateScope())
                {
                    var searchService = scope.ServiceProvider.GetRequiredService<ISemanticSearchService>();
                    await searchService.InitializeCollectionsAsync();
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await ProcessIndexingBatchAsync(stoppingToken);
                        await Task.Delay(batchDelay, stoppingToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogError(ex, "Error processing indexing batch");
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Fatal error in vector index maintenance service");
                throw;
            }
        }

        private async Task ProcessIndexingBatchAsync(CancellationToken stoppingToken)
        {
            var batch = new List<IndexingJob>();

            // Collect batch of jobs
            while (batch.Count < maxBatchSize && indexingQueue.TryDequeue(out var job))
            {
                batch.Add(job);
            }

            if (!batch.Any())
            {
                return;
            }

            // Group jobs by content type for efficient processing
            var jobGroups = batch.GroupBy(j => j.ContentType);
            var tasks = new List<Task>();

            foreach (var group in jobGroups)
            {
                tasks.Add(ProcessJobGroupAsync(group, stoppingToken));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessJobGroupAsync(
            IGrouping<ContentType, IndexingJob> jobs,
            CancellationToken stoppingToken)
        {
            await indexingSemaphore.WaitAsync(stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var searchService = scope.ServiceProvider.GetRequiredService<ISemanticSearchService>();

                foreach (var job in jobs)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        switch (job.Operation)
                        {
                            case IndexOperation.Index:
                                await searchService.IndexContentAsync(
                                    job.ContentId,
                                    job.ContentType,
                                    job.Content);
                                break;

                            case IndexOperation.Update:
                                await searchService.UpdateContentAsync(
                                    job.ContentId,
                                    job.ContentType,
                                    job.Content);
                                break;

                            case IndexOperation.Delete:
                                await searchService.DeleteContentAsync(
                                    job.ContentId,
                                    job.ContentType);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "Error processing indexing job {Operation} for content {ContentId} of type {ContentType}",
                            job.Operation, job.ContentId, job.ContentType);
                    }
                }
            }
            finally
            {
                indexingSemaphore.Release();
            }
        }

        public void EnqueueJob(IndexingJob job)
        {
            indexingQueue.Enqueue(job);
        }

        public class IndexingJob
        {
            public Guid ContentId { get; set; }
            public ContentType ContentType { get; set; }
            public string Content { get; set; }
            public IndexOperation Operation { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public enum IndexOperation
        {
            Index,
            Update,
            Delete
        }
    }
}