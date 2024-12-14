using MetaApi.Models.VirtualFit;
using static System.Net.WebRequestMethods;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        /// <summary>
        /// todo сделать чтоб эти данные загружались из папки
        /// </summary>
        /// <returns></returns>
        public ClothingCollection GetClothingCollection()
        {
            return new ClothingCollection
            {
                //ManItems = new ClothingItem[]
                //{
                //    new ClothingItem { Category = "upper_body", Link = "http://a30944-8332.x.d-f.pw/uploads/28902037-4aa6-4d83-ac59-6cb28fefbc17.PNG" },
                //    new ClothingItem { Category = "upper_body", Link = "http://a30944-8332.x.d-f.pw/uploads/7c1b7ea0-96aa-4aaf-996e-a6dba69cc701.PNG" },
                //    new ClothingItem { Category = "upper_body", Link = "https://a30944-8332.x.d-f.pw/uploads/264f12ce-6b46-46c7-b759-0eb57b2aa8b5.PNG" },
                //    new ClothingItem { Category = "upper_body", Link = "http://a30944-8332.x.d-f.pw/uploads/5f98a6b6-0ce2-4a3a-b827-7a1d67e518aa.png" },
                //    new ClothingItem { Category = "dresses", Link = "http://a30944-8332.x.d-f.pw/uploads/7f1ec457-e101-4612-8631-89e9de712b88.png" },
                //    new ClothingItem { Category = "upper_body", Link = "https://a30944-8332.x.d-f.pw/uploads/ce4e9683-a6dd-4d04-ad83-424023ffe0c4.png" },
                    
                    
                //},

                //WomanItems = new ClothingItem[]
                //{
                //    new ClothingItem { Category = "dresses", Link = "http://a30944-8332.x.d-f.pw/uploads/c0f0442c-9f02-4a85-8859-b6908918eb24.png" },
                //    new ClothingItem { Category = "dresses", Link = "http://a30944-8332.x.d-f.pw/uploads/e5db587d-bc94-4ca5-8dad-6a73f8d8747d.png" },
                //    new ClothingItem { Category = "dresses", Link = "http://a30944-8332.x.d-f.pw/uploads/96f80555-1b79-4fe6-a7f5-838c4bad7794.png" },
                    
                //}
            };
        }
    }
}
