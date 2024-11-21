namespace MetaApi.Models.VirtualFit
{
    public class InputData
    {
        public bool Crop { get; set; }
        public int Seed { get; set; }
        public int Steps { get; set; }

        /// <summary>
        /// dresses, lower_body, upper_body
        /// </summary>
        public string Category { get; set; }
        public bool ForceDc { get; set; }

        /// <summary>
        /// Фото одежды, которую примерить
        /// </summary>
        public string GarmImg { get; set; }

        /// <summary>
        /// Фото человека, на которого примерять
        /// </summary>
        public string HumanImg { get; set; }
        public bool MaskOnly { get; set; }

        /// <summary>
        /// Описание элемента одежды
        /// </summary>
        public string GarmentDes { get; set; }
    }
}
