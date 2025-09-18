using Microsoft.AspNetCore.StaticFiles;

namespace CDD.Api.Helpers
{
    public static class MimeMappingHelper
    {
        private static readonly FileExtensionContentTypeProvider _provider = new();

        /// <summary>
        /// 取得檔案的 MIME 類型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileName)
        {
            if (_provider.TryGetContentType(fileName, out var contentType))
            {
                return contentType;
            }

            // 無法辨識時，回傳預設 octet-stream
            return "application/octet-stream";
        }
    }
}
