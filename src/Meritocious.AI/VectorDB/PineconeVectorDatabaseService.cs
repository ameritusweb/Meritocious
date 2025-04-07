using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinecone;
using Pinecone.Core;
using OneOf;
using Index = Pinecone.Index;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Constants;

namespace Meritocious.AI.VectorDB
{
    public class PineconeVectorDatabaseService : IVectorDatabaseService, IAsyncDisposable
    {
        private readonly ILogger<PineconeVectorDatabaseService> logger;
        private readonly Dictionary<string, Index> indexCache;
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly VectorDBSettings settings;
        private readonly ISecretsService secretsService;
        private BasePinecone? client;

        public PineconeVectorDatabaseService(
           IOptions<VectorDBSettings> settings,
           ISecretsService secretsService,
           ILogger<PineconeVectorDatabaseService> logger)
        {
            this.settings = settings.Value;
            this.secretsService = secretsService;
            this.logger = logger;
            indexCache = new Dictionary<string, Index>();
        }

        private async Task InitializeClientAsync()
        {
            if (client != null)
            {
                return;
            }

            var apiKey = await secretsService.GetSecretAsync(SecretNames.PineconeApiKey);
            var clientOptions = new ClientOptions
            {
                BaseUrl = $"https://controller.{settings.Environment}.pinecone.io",
                HttpClient = new HttpClient(),
                MaxRetries = 3,
                Timeout = TimeSpan.FromSeconds(30),
                IsTlsEnabled = true
            };

            client = new BasePinecone(apiKey, clientOptions);
        }

        public async Task<bool> CreateIndexAsync(string indexName, int dimension)
        {
            try
            {
                await semaphore.WaitAsync();

                await InitializeClientAsync();
                var indexes = await client!.ListIndexesAsync();
                var hasIndex = indexes?.Indexes?.Any(x => x.Name == indexName);
                if (hasIndex.GetValueOrDefault())
                {
                    logger.LogWarning("Index {IndexName} already exists", indexName);
                    return false;
                }

                // Create correct Pod specification
                var podSpec = new PodIndexSpec
                {
                    Pod = new PodSpec
                    {
                        Environment = settings.Environment,
                        PodType = settings.DefaultIndex.PodType,
                        Pods = settings.DefaultIndex.Pods,
                        Replicas = settings.DefaultIndex.Replicas,
                        Shards = settings.DefaultIndex.Shards,
                        SourceCollection = settings.DefaultIndex.SourceCollection,
                        MetadataConfig = new PodSpecMetadataConfig
                        {
                            Indexed = settings.DefaultIndex.MetadataConfig.Indexed
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

                await InitializeClientAsync();
                await client!.CreateIndexAsync(request);
                await WaitForIndexReadyAsync(indexName);

                logger.LogInformation("Created index {IndexName} with dimension {Dimension}",
                    indexName, dimension);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating index {IndexName}", indexName);
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> DeleteIndexAsync(string indexName)
        {
            try
            {
                await semaphore.WaitAsync();

                await InitializeClientAsync();
                var indexes = await client!.ListIndexesAsync();
                var hasIndex = indexes?.Indexes?.Any(x => x.Name == indexName);
                if (!hasIndex.GetValueOrDefault())
                {
                    return false;
                }

                await InitializeClientAsync();
                await client!.DeleteIndexAsync(indexName);
                indexCache.Remove(indexName);

                logger.LogInformation("Deleted index {IndexName}", indexName);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting index {IndexName}", indexName);
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<Index> GetIndexAsync(string indexName)
        {
            try
            {
                if (indexCache.TryGetValue(indexName, out var cachedIndex))
                {
                    return cachedIndex;
                }

                await InitializeClientAsync();
                var index = await client!.DescribeIndexAsync(indexName);
                indexCache[indexName] = index;
                return index;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting index {IndexName}", indexName);
                throw;
            }
        }

        public async Task<List<Index>> ListIndexesAsync()
        {
            try
            {
                await InitializeClientAsync();
                var indexes = await client!.ListIndexesAsync();
                return indexes.Indexes.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error listing indexes");
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

                await InitializeClientAsync();
                await client!.ConfigureIndexAsync(indexName, request);
                indexCache.Remove(indexName); // Clear cache to force refresh

                logger.LogInformation("Configured index {IndexName}", indexName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error configuring index {IndexName}", indexName);
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

                await InitializeClientAsync();
                await client!.Index.UpsertAsync(request);
                logger.LogInformation("Inserted {Count} vectors", vectors.Count);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting vectors");
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

                await InitializeClientAsync();
                await client!.Index.UpsertAsync(request);
                logger.LogInformation("Updated vector {VectorId}", vector.Id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating vector");
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

                await InitializeClientAsync();
                await client!.Index.DeleteAsync(request);
                logger.LogInformation("Deleted {Count} vectors", ids.Count);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting vectors");
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

                await InitializeClientAsync();
                var response = await client!.Index.QueryAsync(request);
                var searchResults = new List<SearchResult>();
                if (response.Results != null)
                {
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
                logger.LogError(ex, "Error searching vectors");
                throw;
            }
        }

        private async Task WaitForIndexReadyAsync(string indexName)
        {
            var timeout = TimeSpan.FromMinutes(5);
            var start = DateTime.UtcNow;

            while (DateTime.UtcNow - start < timeout)
            {
                await InitializeClientAsync();
                var description = await client!.DescribeIndexAsync(indexName);
                if (description.Status.State == IndexModelStatusState.Ready)
                {
                    indexCache[indexName] = description;
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
                logger.LogError(ex, "Error getting index stats for {IndexName}", indexName);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            semaphore.Dispose();
        }

        public Task<bool> CreateCollectionAsync(string collectionName, int dimension)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}