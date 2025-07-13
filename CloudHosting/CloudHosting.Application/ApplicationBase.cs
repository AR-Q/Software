using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CloudHosting.Application
{
    public abstract class ApplicationBase<TResult>
    {
        protected readonly Dictionary<string, object> _metadata = new();

        public TResult Result { get; protected set; }
        public bool IsSuccess { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public IDictionary<string, object> Metadata => _metadata;

        protected ApplicationBase()
        {
            IsSuccess = true;
        }

        protected void SetError(string message)
        {
            IsSuccess = false;
            ErrorMessage = message;
        }

        protected void SetResult(TResult result)
        {
            Result = result;
            IsSuccess = true;
        }

        protected void AddMetadata(string key, object value)
        {
            if (!_metadata.ContainsKey(key))
            {
                _metadata.Add(key, value);
            }
        }
    }

    public abstract class ApplicationService
    {
        protected virtual Task<bool> ValidateAsync()
        {
            return Task.FromResult(true);
        }

        protected virtual void OnBeforeExecute() { }
        protected virtual void OnAfterExecute() { }

        protected async Task<TResponse> ExecuteAsync<TResponse>(Func<Task<TResponse>> action)
            where TResponse : class
        {
            try
            {
                if (!await ValidateAsync())
                {
                    throw new InvalidOperationException("Validation failed");
                }

                OnBeforeExecute();
                var result = await action();
                OnAfterExecute();

                return result;
            }
            catch (Exception ex)
            {
                // Log exception details here
                throw;
            }
        }
    }

    public class ApplicationException : Exception
    {
        public string Code { get; }
        public object[] Parameters { get; }

        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string code, string message, params object[] parameters) 
            : base(message)
        {
            Code = code;
            Parameters = parameters;
        }

        public ApplicationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    public class ValidationException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }
    }
}