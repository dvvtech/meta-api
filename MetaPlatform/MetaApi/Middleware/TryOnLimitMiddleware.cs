//using MetaApi.Services;
//using System.Security.Claims;

//namespace MetaApi.Middleware
//{
//    public class TryOnLimitMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public TryOnLimitMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context, TryOnLimitService limitService)
//        {
//            if (context.Request.Path.StartsWithSegments("/api/tryon"))
//            {
//                var userId = GetCurrentUserId(context); // Реализуйте получение ID пользователя

//                if (!await limitService.CanUserTryOnAsync(userId))
//                {
//                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//                    await context.Response.WriteAsync("Daily try-on limit exceeded");
//                    return;
//                }

//                await _next(context);

//                // Уменьшаем лимит только если запрос успешный
//                if (context.Response.StatusCode < 400)
//                {
//                    await limitService.DecrementTryOnLimitAsync(userId);
//                }
//            }
//            else
//            {
//                await _next(context);
//            }
//        }

//        /// <summary>
//        /// получения ID пользователя из токена
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        private int GetCurrentUserId(HttpContext context)
//        {            
//            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
//            return int.Parse(userIdClaim.Value);
//        }
//    }
//}
