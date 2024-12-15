using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {        
        /// <summary>
        /// todo сделать чтоб эти данные загружались из папки
        /// </summary>
        /// <returns></returns>
        public ClothingCollection GetClothingCollection(string host)
        {            
            return new ClothingCollection
            {
                ManСlothingItems = GetClothingItems(host, FileType.ManСlothing),
                Man = GetClothingItems(host, FileType.Man),
                WomanСlothingItems = GetClothingItems(host, FileType.WomanСlothing),
                Woman = GetClothingItems(host, FileType.Woman),                
            };
        }

        private ClothingItem[] GetClothingItems(string host, FileType fileType)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());
            string[] files = Directory.GetFiles(uploadsPath);
            var clothingItem = new ClothingItem[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                clothingItem[i] = new ClothingItem
                {
                    Link = GenerateFileUrl(fileName, fileType, host),
                };
            }

            return clothingItem;
        }
    }
}
