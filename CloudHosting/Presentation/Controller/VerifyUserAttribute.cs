using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;

public class VerifyUserAttribute : Attribute, IAsyncActionFilter
{
    public static bool isDev = false;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpClient = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("iam");
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<VerifyUserAttribute>>();

        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedObjectResult(new { Message = "Missing or invalid Authorization header." });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var response = new HttpResponseMessage();
            var json = JsonSerializer.Serialize(new { token });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // IF STATEMENT FOR DEV ONLY ENVIORNMENT
            if (isDev)
            {
                response.StatusCode = System.Net.HttpStatusCode.OK;
            }
            else
            {
                response = await httpClient.PostAsync("/api/auth/checkToken", content);
            }

            if (!response.IsSuccessStatusCode)
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Token verification failed." });
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "IAM token verification failed.");
            context.Result = new StatusCodeResult(500);
            return;
        }

        await next();
    }
}
