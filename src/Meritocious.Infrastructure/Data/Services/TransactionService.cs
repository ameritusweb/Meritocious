using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;

namespace Meritocious.Infrastructure.Data.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly MeritociousDbContext context;
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            MeritociousDbContext context,
            ILogger<TransactionService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Result> ExecuteAsync(Func<Task> action)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing transaction");
                await transaction.RollbackAsync();
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing transaction");
                await transaction.RollbackAsync();
                return Result.Failure<T>(ex.Message);
            }
        }
    }
}