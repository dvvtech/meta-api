﻿namespace MetaApi.Models.VirtualFit
{

    public enum FileType : byte
    {
        None = 0,

        ManСlothing = 1,

        WomanСlothing = 2,

        Man = 3,

        Woman = 4,

        Upload = 5,

        Result = 6
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
                FileType.ManСlothing => "manClothing",
                FileType.WomanСlothing => "womanClothing",                         
                FileType.Man => "man",
                FileType.Woman => "woman",
                FileType.Upload => "uploads",
                FileType.Result => "result",
                _ => "unknown" // По умолчанию, если FileType не определён
            };
        }
    }
}
