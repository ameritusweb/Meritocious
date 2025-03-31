using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinecone;
using Pinecone.Core;
using OneOf;
using Index = Pinecone.Index;

namespace Meritocious.AI.VectorDB
{
    public class PineconeVectorDatabaseService : IVectorDatabaseService, IAsyncDisposable
    {
        private readonly BasePinecone _client;
        private readonly ILogger<PineconeVectorDatabaseService> _logger;
        private readonly Dictionary<string, Index> _indexCache;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly VectorDBSettings _settings;

        public PineconeVectorDatabaseService(
           IOptions<VectorDBSettings> settings,
           ILogger<PineconeVectorDatabaseService> logger)
        {
            _settings = settings.Value;

            var clientOptions = new ClientOptions
            {
                BaseUrl = $"https://controller.{_settings.Environment}.pinecone.io",
                HttpClient = new HttpClient(),
                MaxRetries = 3,
                Timeout = TimeSpan.FromSeconds(30),
                IsTlsEnabled = true
            };

            _client = new BasePinecone(_settings.ApiKey, clientOptions);
            _logger = logger;
            _indexCache = new Dictionary<string, Index>();
        }

        public async Task<bool> CreateIndexAsync(string indexName, int dimension)
        {
            try
            {
                await _semaphore.WaitAsync();

                var indexes = await _client.ListIndexesAsync();
                var hasIndex = indexes?.Indexes?.Any(x => x.Name == indexName);
                if (hasIndex.GetValueOrDefault())
                {
                    _logger.LogWarning("Index {IndexName} already exists", indexName);
                    return false;
                }

                // Create correct Pod specification
                var podSpec = new PodIndexSpec
                {
                    Pod = new PodSpec
                    {
                        Environment = _settings.Environment,
                        PodType = _settings.DefaultIndex.PodType,
                        Pods = _settings.DefaultIndex.Pods,
                        Replicas = _settings.DefaultIndex.Replicas,
                        Shards = _settings.DefaultIndex.Shards,
                        SourceCollection = _settings.DefaultIndex.SourceCollection,
                        MetadataConfig = new PodSpecMetadataConfig
                        {
                            Indexed = _settings.DefaultIndex.MetadataConfig.Indexed
                        }
                    }
                };

                var request = new CreateIndexRequest
                {
                    Name = indexName,
                    Dimension = dimension,
                    Metric = CreateIndexRequestMetric.Cosine,
                    Spec = OneOf<ServerlessIndexSpec, PodIndexSpec>.FromT1(podSpec)
                };

                await _client.CreateIndexAsync(request);
                await WaitForIndexReadyAsync(indexName);

                _logger.LogInformation("Created index {IndexName} with dimension {Dimension}",
                    indexName, dimension);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating index {IndexName}", indexName);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> DeleteIndexAsync(string indexName)
        {
            try
            {
                await _semaphore.WaitAsync();

                var indexes = await _client.ListIndexesAsync();
                var hasIndex = indexes?.Indexes?.Any(x => x.Name == indexName);
                if (!hasIndex.GetValueOrDefault())
                {
                    return false;
                }

                await _client.DeleteIndexAsync(indexName);
                _indexCache.Remove(indexName);

                _logger.LogInformation("Deleted index {IndexName}", indexName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting index {IndexName}", indexName);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Index> GetIndexAsync(string indexName)
        {
            try
            {
                if (_indexCache.TryGetValue(indexName, out var cachedIndex))
                {
                    return cachedIndex;
                }

                var index = await _client.DescribeIndexAsync(indexName);
                _indexCache[indexName] = index;
                return index;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting index {IndexName}", indexName);
                throw;
            }
        }

        public async Task<List<Index>> ListIndexesAsync()
        {
            try
            {
                var indexes = await _client.ListIndexesAsync();
                return indexes.Indexes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing indexes");
                throw;
            }
        }

        public async Task ConfigureIndexAsync(string indexName, IndexConfigurationOptions options)
        {
            try
            {
                var request = new ConfigureIndexRequest();

                // Only create and set Spec if we have pod configuration options
                if (options.Replicas.HasValue || !string.IsNullOrEmpty(options.PodType))
                {
                    request.Spec = new ConfigureIndexRequestSpec
                    {
                        Pod = new ConfigureIndexRequestSpecPod
                        {
                            Replicas = options.Replicas,
                            PodType = options.PodType
                        }
                    };
                }

                // Configure deletion protection if specified
                if (options.DeletionProtection.HasValue)
                {
                    request.DeletionProtection = options.DeletionProtection;
                }

                // Add tags if provided
                if (options.Tags != null && options.Tags.Count > 0)
                {
                    request.Tags = options.Tags;
                }

                // Configure embedding if specified
                if (options.EmbeddingConfig != null)
                {
                    request.Embed = new ConfigureIndexRequestEmbed
                    {
                        Model = options.EmbeddingConfig.Model,
                        FieldMap = options.EmbeddingConfig.FieldMap,
                        ReadParameters = options.EmbeddingConfig.ReadParameters,
                        WriteParameters = options.EmbeddingConfig.WriteParameters
                    };
                }

                await _client.ConfigureIndexAsync(indexName, request);
                _indexCache.Remove(indexName); // Clear cache to force refresh

                _logger.LogInformation("Configured index {IndexName}", indexName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring index {IndexName}", indexName);
                throw;
            }
        }

        public async Task<bool> InsertVectorsAsync(string indexName, List<VectorEntry> vectors)
        {
            try
            {
                var request = new UpsertRequest
                {
                    Vectors = vectors.Select(v => v.ToPineconeVector()).ToList()
                };

                await _client.Index.UpsertAsync(request);
                _logger.LogInformation("Inserted {Count} vectors", vectors.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting vectors");
                throw;
            }
        }

        public async Task<bool> UpdateVectorAsync(string indexName, VectorEntry vector)
        {
            try
            {
                var request = new UpsertRequest
                {
                    Vectors = new[] { vector.ToPineconeVector() }
                };

                await _client.Index.UpsertAsync(request);
                _logger.LogInformation("Updated vector {VectorId}", vector.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vector");
                throw;
            }
        }

        public async Task<bool> DeleteVectorsAsync(string indexName, List<string> ids)
        {
            try
            {
                var request = new DeleteRequest
                {
                    Ids = ids
                };

                await _client.Index.DeleteAsync(request);
                _logger.LogInformation("Deleted {Count} vectors", ids.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vectors");
                throw;
            }
        }

        public async Task<List<SearchResult>> SearchAsync(
            string indexName,
            float[] queryVector,
            int topK = 10,
            SearchFilter filter = null)
        {
            try
            {
                var request = new QueryRequest
                {
                    TopK = (uint)topK,
                    Vector = new ReadOnlyMemory<float>(queryVector),
                    IncludeValues = true,
                    IncludeMetadata = true
                };

                if (filter != null)
                {
                    request.Filter = filter.ToMetadata();
                }

                var response = await _client.Index.QueryAsync(request);
                var searchResults = new List<SearchResult>();
                if (response.Results != null) {
                    foreach (var results in response.Results)
                    {
                        if (results.Matches != null)
                        {
                            foreach (var match in results.Matches)
                            {
                                searchResults.Add(SearchResult.FromPineconeMatch(match));
                            }
                        }
                    }
                }

                return searchResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching vectors");
                throw;
            }
        }

        private async Task WaitForIndexReadyAsync(string indexName)
        {
            var timeout = TimeSpan.FromMinutes(5);
            var start = DateTime.UtcNow;

            while (DateTime.UtcNow - start < timeout)
            {
                var description = await _client.DescribeIndexAsync(indexName);
                if (description.Status.State == IndexModelStatusState.Ready)
                {
                    _indexCache[indexName] = description;
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            throw new TimeoutException($"Index {indexName} did not become ready within timeout period");
        }

        public async Task<IndexStats> GetIndexStatsAsync(string indexName)
        {
            try
            {
                var index = await GetIndexAsync(indexName);
                return new IndexStats
                {
                    Dimension = index.Dimension ?? 0,
                    TotalVectorCount = 0, // Would need to use DescribeIndexStats API if available
                    Namespace = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting index stats for {IndexName}", indexName);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            _semaphore.Dispose();
        }
    }
}