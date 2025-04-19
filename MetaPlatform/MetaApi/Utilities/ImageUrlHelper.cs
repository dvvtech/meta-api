using MetaApi.Consts;

namespace MetaApi.Utilities
{
    public static class ImageUrlHelper
    {
        public static string GetUrl(string url)
        {
            string imgFileName = Path.GetFileNameWithoutExtension(url);
            var underscoreCount = imgFileName.Count(c => c == '_');
            if (underscoreCount == 1)
            {
                //todo если оканчивается на _t то заменить на _v
                if (imgFileName.EndsWith(FittingConstants.THUMBNAIL_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
                {
                    return url.Replace(FittingConstants.THUMBNAIL_SUFFIX_URL, FittingConstants.FULLSIZE_SUFFIX_URL);
                }

                return url;
            }

            //если фото из внутренней коллекции и для него есть паддинг, то нужно заменить название файла, чтоб оканчивался на _p
            if (imgFileName.EndsWith(FittingConstants.FULLSIZE_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.PADDING_SUFFIX_URL);
            }

            //если фото из внутренней коллекции 
            if (imgFileName.EndsWith(FittingConstants.THUMBNAIL_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace(FittingConstants.THUMBNAIL_SUFFIX_URL, FittingConstants.PADDING_SUFFIX_URL);
            }

            throw new InvalidOperationException("Invalid image URL format");
        }
    }
}
