using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Core.OperationResults
{
    public static class ClaimsError
    {
        public const string ClaimUserIdNotFoundCode = "Claim.UserIdNotFound";
        public const string ClaimUserIdFailedParseCode = "Claim.UserIdFailedParse";

        public static Error ClaimUserIdNotFound() => new Error(ClaimUserIdNotFoundCode, "user id not found");

        public static Error ClaimUserIdFailedParse(string userId) => new Error(ClaimUserIdFailedParseCode,
                                                                     $"failed parse user id, source userId:{userId}");
    }
}
