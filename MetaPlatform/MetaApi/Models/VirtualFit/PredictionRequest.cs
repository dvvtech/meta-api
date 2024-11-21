using Microsoft.AspNetCore.Components.Forms;

namespace MetaApi.Models.VirtualFit
{
    public class PredictionRequest
    {
        /// <summary>
        /// Версия ИИ модели
        /// </summary>
        public string Version { get; set; }

        public InputData Input { get; set; }
    }
}
