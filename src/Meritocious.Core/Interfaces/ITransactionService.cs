using Meritocious.Core.Results;

namespace Meritocious.Core.Interfaces
{
    public interface ITransactionService
    {
        /// <summary>
        /// Executes the given action within a transaction.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns>A Result indicating success or failure</returns>
        Task<Result> ExecuteAsync(Func<Task> action);

        /// <summary>
        /// Executes the given action within a transaction and returns a result.
        /// </summary>
        /// <typeparam name="T">The type of the result</typeparam>
        /// <param name="action">The action to execute</param>
        /// <returns>A Result containing the action's result</returns>
        Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action);
    }
}