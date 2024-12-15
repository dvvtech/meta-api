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
            if (Directory.Exists(uploadsPath))
            {
                var clothingItem = new List<ClothingItem>();
                string[] files = Directory.GetFiles(uploadsPath);
                foreach(string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    bool endsWith_T = fileName.Contains("_t.") && fileName.Substring(0, fileName.LastIndexOf('.')).EndsWith("_t");
                    if (endsWith_T)
                    {
                        clothingItem.Add(new ClothingItem
                        {
                            Link = GenerateFileUrl(fileName, fileType, host),
                        });
                    }
                }

                return clothingItem.ToArray();

                /*var clothingItem = new ClothingItem[files.Length/2];
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);
                    bool endsWith_T = fileName.Contains("_t.") && fileName.Substring(0, fileName.LastIndexOf('.')).EndsWith("_t");
                    if (endsWith_T)
                    {
                        clothingItem[i] = new ClothingItem
                        {
                            Link = GenerateFileUrl(fileName, fileType, host),
                        };
                    }
                }
                return clothingItem;*/
            }

            return new ClothingItem[0];
        }
    }
}
