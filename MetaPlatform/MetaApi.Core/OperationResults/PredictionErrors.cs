using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Core.OperationResults
{
    public class PredictionErrors
    {        
        public static Error PredictionRequestFail(string httpStatusCode) => new Error(httpStatusCode, "Prediction request fail");

        public static Error PredictionStatusFail(string httpStatusCode) => new Error(httpStatusCode, "Prediction status fail");

        public static Error PredictionImageEmpty() => new Error("500", "Prediction is image empty");

        public static Error PredictionProcessFail(string description) => new Error("400", $"Prediction process fail: {description}");

        public static Error PredictionTimeout() => new Error("504", "Prediction timeout");

        public static Error PredictionException(string description) => new Error("500", $"Prediction Exception: {description}");
    }
}
