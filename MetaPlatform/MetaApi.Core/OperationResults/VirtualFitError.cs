using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Core.OperationResults
{
    public static class VirtualFitError
    {
        public const string ThirdPartyServiceErrorCode = "ThirdPartyService.Error";

        public const string VirtualFitServiceErrorCode = "CurrentService.Error";

        public static Error ThirdPartyServiceError(string info) => new Error(ThirdPartyServiceErrorCode, $"{info}");

        public static Error VirtualFitServiceError(string info) => new Error(VirtualFitServiceErrorCode, $"{info}");
    }
}
