using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MetaApi.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static Result<string> GetCurrentUserId(this ControllerBase apiController)
        {
            var identifier = apiController.User.FindFirst(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identifier?.Value))
            {
                return Result<string>.Failure(ClaimsError.ClaimUserIdNotFound());
            }

            return Result<string>.Success(identifier.Value);
        }
    }
}
