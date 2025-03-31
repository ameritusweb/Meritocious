using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Behaviors
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly DbContext _dbContext;

        public TransactionBehavior(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                return await next();
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();
                await transaction.CommitAsync(cancellationToken);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}