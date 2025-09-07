namespace MetaApi.Models.VirtualFit
{

    public enum FileType : byte
    {
        None = 0,

        ManСlothing = 1,

        WomanСlothing = 2,

        Man = 3,

        Woman = 4,

        Upload = 5,

        Result = 6,

        Examples = 7,

        Other = 8,

        ManHair = 9,

        WomanHair = 10,

        HairExamples = 11
    }

    public static class FileTypeExtensions
    {
        /// <summary>
        /// Возвращает название папки на основе значения FileType.
        /// </summary>
        public static string GetFolderName(this FileType fileType)
        {
            return fileType switch
            {
                FileType.None => "none",
                FileType.ManСlothing => "manClothing",
                FileType.WomanСlothing => "womanClothing",                         
                FileType.Man => "man",
                FileType.Woman => "woman",
                FileType.Upload => "uploads",
                FileType.Result => "result",
                FileType.Examples => "examples",
                FileType.Other => "other",
                FileType.ManHair => "manHair",
                FileType.WomanHair => "womanHair",
                FileType.HairExamples => "hairExamples",
                _ => "unknown" // По умолчанию, если FileType не определён
            };
        }
    }
}
