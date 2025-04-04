using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Results
{
    public static class ResultExtensions
    {
        public static Result<TOut> Map<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> mapper)
        {
            if (result.IsFailure)
            {
                return Result.Failure<TOut>(result.Error);
            }

            return Result.Success(mapper(result.Value));
        }

        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, Task<TOut>> mapper)
        {
            var result = await resultTask;
            if (result.IsFailure)
            {
                return Result.Failure<TOut>(result.Error);
            }

            var mappedValue = await mapper(result.Value);
            return Result.Success(mappedValue);
        }

        public static Result<T> Ensure<T>(
            this Result<T> result,
            Func<T, bool> predicate,
            string errorMessage)
        {
            if (result.IsFailure)
            {
                return result;
            }

            return predicate(result.Value)
                ? result
                : Result.Failure<T>(errorMessage);
        }

        public static Result<TOut> Bind<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Result<TOut>> func)
        {
            if (result.IsFailure)
            {
                return Result.Failure<TOut>(result.Error);
            }

            return func(result.Value);
        }

        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, Task<Result<TOut>>> func)
        {
            var result = await resultTask;
            if (result.IsFailure)
            {
                return Result.Failure<TOut>(result.Error);
            }

            return await func(result.Value);
        }

        public static Result<T> Tap<T>(
            this Result<T> result,
            Action<T> action)
        {
            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }

        public static async Task<Result<T>> TapAsync<T>(
            this Task<Result<T>> resultTask,
            Func<T, Task> action)
        {
            var result = await resultTask;
            if (result.IsSuccess)
            {
                await action(result.Value);
            }

            return result;
        }
    }
}
