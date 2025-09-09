using MetaApi.Core.Domain.Hair;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class FileService
    {
        /// <summary>
        /// todo сделать чтоб эти данные загружались из папки
        /// </summary>
        /// <returns></returns>
        public HairCollection GetHairCollection(string host)
        {
            return new HairCollection
            {                
                Man = GetHairItems(host, FileType.ManHair),                
                Woman = GetHairItems(host, FileType.WomanHair),
            };
        }

        private HairItem[] GetHairItems(string host, FileType fileType)
        {
            var uploadsPath = Path.Combine(_webRootPath, fileType.GetFolderName());
            if (Directory.Exists(uploadsPath))
            {
                var clothingItem = new List<HairItem>();
                string[] files = Directory.GetFiles(uploadsPath);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    bool endsWith_T = fileName.Contains("_t.") && fileName.Substring(0, fileName.LastIndexOf('.')).EndsWith("_t");
                    if (endsWith_T)
                    {
                        clothingItem.Add(new HairItem
                        {
                            Link = GenerateFileUrl(fileName, fileType, host),
                        });
                    }
                }

                return clothingItem.ToArray();
            }

            return new HairItem[0];
        }
    }
}
