using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        protected bool HasError => !string.IsNullOrEmpty(Error);

        protected Result(bool isSuccess, string error = null)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("A successful result cannot have an error");
            }

            if (!isSuccess && string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("A failure result must have an error");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true);
        public static Result Failure(string error) => new Result(false, error);
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("Cannot access value of a failed result");
                }

                return value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error = null)
            : base(isSuccess, error)
        {
            this.value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true);
        public new static Result<T> Failure(string error) => new Result<T>(default, false, error);
    }
}