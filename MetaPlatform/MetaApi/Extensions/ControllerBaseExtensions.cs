using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MetaApi.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static Result<int> GetCurrentUserId(this ControllerBase apiController)
        {
            var identifier = apiController.User.FindFirst(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identifier?.Value))
            {
                return Result<int>.Failure(ClaimsError.ClaimUserIdNotFound());
            }

            var userId = int.Parse(identifier?.Value);

            return Result<int>.Success(userId);
        }
    }
}
