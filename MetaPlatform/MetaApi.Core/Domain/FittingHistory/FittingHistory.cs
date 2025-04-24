
namespace MetaApi.Core.Domain.FittingHistory
{
    public class FittingHistory
    {
        public int Id { get; set; }

        public string GarmentImgUrl { get; set; }

        public string HumanImgUrl { get; set; }

        public string ResultImgUrl { get; set; }

        public int AccountId { get; set; }

        private FittingHistory(int accountId, string garmentImgUrl, string humanImgUrl, string resultImgUrl)
        {            
            AccountId = accountId;
            GarmentImgUrl = garmentImgUrl;
            HumanImgUrl = humanImgUrl;
            ResultImgUrl = resultImgUrl;
        }

        private FittingHistory(int id, int accountId, string garmentImgUrl, string humanImgUrl, string resultImgUrl)
        {
            Id = id;
            AccountId = accountId;
            GarmentImgUrl = garmentImgUrl;
            HumanImgUrl = humanImgUrl;
            ResultImgUrl = resultImgUrl;
        }


        public static FittingHistory Create(int accountId, string garmentImgUrl, string humanImgUrl, string resultImgUrl)
        {
            return new FittingHistory(accountId, garmentImgUrl, humanImgUrl, resultImgUrl);
        }

        public static FittingHistory Create(int id, int accountId, string garmentImgUrl, string humanImgUrl, string resultImgUrl)
        {
            return new FittingHistory(id, accountId, garmentImgUrl, humanImgUrl, resultImgUrl);
        }
    }
}
