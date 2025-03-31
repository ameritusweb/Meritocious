using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Meritocious.Infrastructure.Data.Repositories;
using System.Collections.Concurrent;
using Meritocious.Common.Enums;

namespace Meritocious.AI.Search
{
    public class VectorIndexMaintenanceService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<VectorIndexMaintenanceService> _logger;
        private readonly ConcurrentQueue<IndexingJob> _indexingQueue;
        private readonly SemaphoreSlim _indexingSemaphore;
        private readonly TimeSpan _batchDelay = TimeSpan.FromSeconds(5);
        private readonly int _maxBatchSize = 100;
        private readonly int _maxConcurrentIndexing = 3;

        public VectorIndexMaintenanceService(
            IServiceScopeFactory scopeFactory,
            ILogger<VectorIndexMaintenanceService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _indexingQueue = new ConcurrentQueue<IndexingJob>();
            _indexingSemaphore = new SemaphoreSlim(_maxConcurrentIndexing);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Initialize vector collections
                using (var scope = _scopeFactory.CreateScope())
                {
                    var searchService = scope.ServiceProvider.GetRequiredService<ISemanticSearchService>();
                    await searchService.InitializeCollectionsAsync();
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await ProcessIndexingBatchAsync(stoppingToken);
                        await Task.Delay(_batchDelay, stoppingToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogError(ex, "Error processing indexing batch");
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Fatal error in vector index maintenance service");
                throw;
            }
        }

        private async Task ProcessIndexingBatchAsync(CancellationToken stoppingToken)
        {
            var batch = new List<IndexingJob>();

            // Collect batch of jobs
            while (batch.Count < _maxBatchSize && _indexingQueue.TryDequeue(out var job))
            {
                batch.Add(job);
            }

            if (!batch.Any()) return;

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
            await _indexingSemaphore.WaitAsync(stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var searchService = scope.ServiceProvider.GetRequiredService<ISemanticSearchService>();

                foreach (var job in jobs)
                {
                    if (stoppingToken.IsCancellationRequested) break;

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
                        _logger.LogError(ex,
                            "Error processing indexing job {Operation} for content {ContentId} of type {ContentType}",
                            job.Operation, job.ContentId, job.ContentType);
                    }
                }
            }
            finally
            {
                _indexingSemaphore.Release();
            }
        }

        public void EnqueueJob(IndexingJob job)
        {
            _indexingQueue.Enqueue(job);
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