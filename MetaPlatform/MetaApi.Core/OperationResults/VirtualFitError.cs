﻿using MetaApi.Core.OperationResults.Base;
using System.Net;

namespace MetaApi.Core.OperationResults
{
    public static class VirtualFitError
    {
        public const string ThirdPartyServiceErrorCode = "ThirdPartyService.Error";

        public const string VirtualFitServiceErrorCode = "CurrentService.Error";        

        public const string LimitIsOverErrorCode = "LimitIsOver.Error";        

        public static Error ThirdPartyServiceError(string info, HttpStatusCode httpStatusCode) => new Error(httpStatusCode.ToString(), $"{info}");

        public static Error VirtualFitServiceError(string info) => new Error(VirtualFitServiceErrorCode, $"{info}");

        public static Error LimitIsOverError() => new Error(LimitIsOverErrorCode, "Limit is over");
    }
}
