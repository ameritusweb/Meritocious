using Meritocious.AI.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinecone;
using Index = Pinecone.Index;
using Vector = Pinecone.Vector;

namespace Meritocious.AI.VectorDB
{
    public class PineconeVectorDatabaseService : IVectorDatabaseService, IAsyncDisposable
    {
        private readonly PineconeClient _client;
        private readonly ILogger<PineconeVectorDatabaseService> _logger;
        private readonly Dictionary<string, Index> _indexCache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PineconeVectorDatabaseService(
            IOptions<VectorDBSettings> settings,
            ILogger<PineconeVectorDatabaseService> logger)
        {
            _client = new PineconeClient(settings.Value.ApiKey, new ClientOptions
            {
                Environment = settings.Value.Environment
            });
            _logger = logger;
            _indexCache = new Dictionary<string, Index>();
        }

        public async Task<bool> CreateCollectionAsync(string collectionName, int dimension)
        {
            try
            {
                await _semaphore.WaitAsync();

                if (await CollectionExistsAsync(collectionName))
                {
                    _logger.LogWarning("Index {CollectionName} already exists", collectionName);
                    return false;
                }

                // Create index
                await _client.CreateIndexAsync(new CreateIndexRequest
                {
                    Name = collectionName,
                    Dimension = dimension,
                    Metric = "cosine",
                    Pods = 1,
                    PodType = "p1.x1" // Adjust based on your needs
                });

                // Wait for index to be ready
                await WaitForIndexReadyAsync(collectionName);

                // Cache the index instance
                var index = _client.Index(collectionName);
                _indexCache[collectionName] = index;

                _logger.LogInformation("Created index {CollectionName} with dimension {Dimension}",
                    collectionName, dimension);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating index {CollectionName}", collectionName);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> DeleteCollectionAsync(string collectionName)
        {
            try
            {
                await _semaphore.WaitAsync();

                if (!await CollectionExistsAsync(collectionName))
                {
                    return false;
                }

                await _client.DeleteIndexAsync(collectionName);
                _indexCache.Remove(collectionName);

                _logger.LogInformation("Deleted index {CollectionName}", collectionName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting index {CollectionName}", collectionName);
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> InsertVectorsAsync(string collectionName, List<VectorEntry> vectors)
        {
            try
            {
                var index = await GetIndexAsync(collectionName);

                var upsertRequest = new UpsertRequest
                {
                    Vectors = vectors.Select(v => new Vector
                    {
                        Id = v.Id,
                        Values = v.Vector,
                        Metadata = v.Metadata
                    }).ToList()
                };

                await index.UpsertAsync(upsertRequest);

                _logger.LogInformation("Inserted {Count} vectors into index {CollectionName}",
                    vectors.Count, collectionName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting vectors into index {CollectionName}",
                    collectionName);
                throw;
            }
        }

        public async Task<List<SearchResult>> SearchAsync(
            string collectionName,
            float[] queryVector,
            int topK = 10,
            SearchFilter filter = null)
        {
            try
            {
                var index = await GetIndexAsync(collectionName);

                var request = new QueryRequest
                {
                    TopK = topK,
                    Vector = queryVector,
                    IncludeMetadata = true
                };

                if (filter != null)
                {
                    request.Filter = new Dictionary<string, object>
                    {
                        { filter.FieldName, filter.Value }
                    };
                }

                var response = await index.QueryAsync(request);

                return response.Matches.Select(m => new SearchResult
                {
                    Id = m.Id,
                    Score = m.Score,
                    Metadata = m.Metadata,
                    Vector = m.Values
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching in index {CollectionName}", collectionName);
                throw;
            }
        }

        public async Task<bool> DeleteVectorsAsync(string collectionName, List<string> ids)
        {
            try
            {
                var index = await GetIndexAsync(collectionName);
                await index.DeleteAsync(new DeleteRequest { Ids = ids });

                _logger.LogInformation("Deleted {Count} vectors from index {CollectionName}",
                    ids.Count, collectionName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vectors from index {CollectionName}",
                    collectionName);
                throw;
            }
        }

        public async Task<bool> UpdateVectorAsync(string collectionName, VectorEntry vector)
        {
            try
            {
                var index = await GetIndexAsync(collectionName);

                var upsertRequest = new UpsertRequest
                {
                    Vectors = new List<Vector>
                    {
                        new Vector
                        {
                            Id = vector.Id,
                            Values = vector.Vector,
                            Metadata = vector.Metadata
                        }
                    }
                };

                await index.UpsertAsync(upsertRequest);

                _logger.LogInformation("Updated vector {VectorId} in index {CollectionName}",
                    vector.Id, collectionName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vector in index {CollectionName}",
                    collectionName);
                throw;
            }
        }

        private async Task<bool> CollectionExistsAsync(string collectionName)
        {
            if (_indexCache.ContainsKey(collectionName))
                return true;

            var indexes = await _client.ListIndexesAsync();
            return indexes.Contains(collectionName);
        }

        private async Task<Index> GetIndexAsync(string collectionName)
        {
            if (_indexCache.TryGetValue(collectionName, out var index))
                return index;

            if (!await CollectionExistsAsync(collectionName))
            {
                throw new InvalidOperationException($"Index {collectionName} does not exist");
            }

            index = _client.Index(collectionName);
            _indexCache[collectionName] = index;
            return index;
        }

        private async Task WaitForIndexReadyAsync(string indexName)
        {
            var timeout = TimeSpan.FromMinutes(5);
            var start = DateTime.UtcNow;

            while (DateTime.UtcNow - start < timeout)
            {
                var description = await _client.DescribeIndexAsync(indexName);
                if (description.Status.State == "Ready")
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            throw new TimeoutException($"Index {indexName} did not become ready within timeout period");
        }

        public async ValueTask DisposeAsync()
        {
            _semaphore.Dispose();
        }
    }
}