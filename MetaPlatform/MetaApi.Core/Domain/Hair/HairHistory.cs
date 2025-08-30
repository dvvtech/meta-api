
namespace MetaApi.Core.Domain.Hair
{
    public class HairHistory
    {
        public int Id { get; set; }

        /// <summary>
        /// Фото прически
        /// </summary>
        public string HairImg { get; set; }

        /// <summary>
        /// Фото, на кого примерить прическу
        /// </summary>
        public string FaceImg { get; set; }

        /// <summary>
        /// Результат примерки прически
        /// </summary>
        public string ResultImgUrl { get; set; }

        public int AccountId { get; set; }

        private HairHistory(int accountId, string hairImgImgUrl, string faceImgUrl, string resultImgUrl)
        {
            AccountId = accountId;
            HairImg = hairImgImgUrl;
            FaceImg = faceImgUrl;
            ResultImgUrl = resultImgUrl;
        }

        private HairHistory(int id, int accountId, string hairImgImgUrl, string faceImgUrl, string resultImgUrl)
        {
            Id = id;
            AccountId = accountId;
            HairImg = hairImgImgUrl;
            FaceImg = faceImgUrl;
            ResultImgUrl = resultImgUrl;
        }


        public static HairHistory Create(int accountId, string hairImgImgUrl, string faceImgUrl, string resultImgUrl)
        {
            return new HairHistory(accountId, hairImgImgUrl, faceImgUrl, resultImgUrl);
        }

        public static HairHistory Create(int id, int accountId, string hairImgImgUrl, string faceImgUrl, string resultImgUrl)
        {
            return new HairHistory(id, accountId, hairImgImgUrl, faceImgUrl, resultImgUrl);
        }
    }
}
