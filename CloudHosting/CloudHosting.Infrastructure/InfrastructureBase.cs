using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CloudHosting.Infrastructure
{
    public abstract class InfrastructureBase
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;

        protected InfrastructureBase(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected virtual void LogError(Exception ex, string message = null)
        {
            _logger.LogError(ex, message ?? ex.Message);
        }

        protected virtual void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        protected T GetConfiguration<T>(string key, T defaultValue = default)
        {
            return _configuration.GetValue(key, defaultValue);
        }
    }

    public abstract class InfrastructureService : InfrastructureBase
    {
        protected InfrastructureService(ILogger logger, IConfiguration configuration) 
            : base(logger, configuration)
        {
        }

        protected virtual async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxRetries = 3)
        {
            var attempts = 0;
            while (attempts < maxRetries)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    attempts++;
                    if (attempts == maxRetries)
                        throw;

                    LogError(ex, $"Retry attempt {attempts} of {maxRetries}");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts))); // Exponential backoff
                }
            }
            throw new InvalidOperationException("Should not reach here");
        }
    }

    public class InfrastructureException : Exception
    {
        public string ErrorCode { get; }

        public InfrastructureException(string message) : base(message)
        {
        }

        public InfrastructureException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public InfrastructureException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}