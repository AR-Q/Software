using CloudHosting.Core.Interfaces;
using System.Text;
using System.Text.Json;

namespace CloudHosting.Infrastructure.Services
{
    public class IamService(HttpClient httpClient, ILogger<IamService> logger) : IIamService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<IamService> _logger = logger;

        public async Task<string> GetUserIdFromTokenAsync(string token)
        {
            try
            {
                // Create request body
                var requestBody = JsonSerializer.Serialize(new { Token = token });
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send request
                var response = await _httpClient.PostAsync("/api/auth/verify", content);
                response.EnsureSuccessStatusCode();

                // Parse response
                var responseJson = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseJson);
                var userId = jsonDocument.RootElement.GetProperty("UserId").GetString();
                
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Token validation succeeded but UserId is null or empty in the response");
                }
                
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate token with IAM service");
                throw;
            }
        }
    }
}