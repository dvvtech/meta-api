using MetaApi.Services;
using System.Security.Claims;

namespace MetaApi.Middleware
{
    public class TryOnLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public TryOnLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TryOnLimitService limitService)
        {
            if (context.Request.Path.StartsWithSegments("/api/virtual-fit/try-on"))
            {
                var userId = GetCurrentUserId(context);

                if (!await limitService.CanUserTryOnAsync(userId))
                {
                    var timeRemaining = await limitService.GetTimeUntilLimitResetAsync(userId);

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    var message = $"Try-on limit exceeded. Please try again in {FormatTimeRemaining(timeRemaining)}.";
                    await context.Response.WriteAsync(message);

                    return;
                }

                await _next(context);

            }
            else
            {
                await _next(context);
            }
        }

        /// <summary>
        /// получения ID пользователя из токена
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private int GetCurrentUserId(HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        private string FormatTimeRemaining(TimeSpan timeRemaining)
        {
            var parts = new List<string>();

            if (timeRemaining.Days > 0)
                parts.Add($"{timeRemaining.Days} day(s)");

            if (timeRemaining.Hours > 0)
                parts.Add($"{timeRemaining.Hours} hour(s)");

            if (timeRemaining.Minutes > 0)
                parts.Add($"{timeRemaining.Minutes} minute(s)");

            if (timeRemaining.Seconds > 0 || parts.Count == 0) // Всегда показываем хотя бы секунды
                parts.Add($"{timeRemaining.Seconds} second(s)");

            // Соединяем все части с правильными разделителями
            if (parts.Count == 1)
                return parts[0];

            var allExceptLast = string.Join(", ", parts.Take(parts.Count - 1));
            return $"{allExceptLast} and {parts.Last()}";
        }
    }
}
