using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;


namespace Sample.Api.Helpers
{
    public static class ImageBase64ResizeHelper
    {
        private static readonly string[] SupportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif" };
        private static readonly string[] UnsupportedByImageSharp = new[] { ".ico" };

        /// <summary>
        /// 建立 image 縮圖
        /// </summary>
        /// <param name="base64Input"></param>
        /// <param name="fileName"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static async Task<string?> ResizeBase64ImageAsync(string base64Input, string fileName, int maxWidth, int maxHeight)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (!IsImage(extension) && !IsUnsupportedImage(extension))
            {
                return null; // 非圖片不處理
            }

            // ICO 等格式暫不支援縮圖功能
            if (IsUnsupportedImage(extension))
            {
                return base64Input; // 直接返回原始 Base64，不進行縮圖處理
            }

            try
            {
                string sanitizedBase64 = SanitizeBase64(base64Input);
                byte[] imageBytes = Convert.FromBase64String(sanitizedBase64);

                // 檢查是否有足夠的資料
                if (imageBytes.Length < 10)
                {
                    throw new InvalidOperationException($"圖片資料太小: {imageBytes.Length} bytes");
                }

                // 檢查檔案頭以確認圖片格式
                string extensionFromImageBytes = string.Empty;
                if (!IsValidImageHeader(imageBytes, out extensionFromImageBytes))
                {
                    throw new InvalidOperationException($"無效的圖片格式，檔案頭: {BitConverter.ToString(imageBytes.Take(8).ToArray())}");
                }

                if (IsUnsupportedImage(extensionFromImageBytes))
                {
                    return base64Input; // 直接返回原始 Base64，不進行縮圖處理
                }

                using (var inputStream = new MemoryStream(imageBytes))
                {
                    inputStream.Position = 0; // 確保 stream 位置在開頭
                    using (Image image = await Image.LoadAsync(inputStream))
                    {

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(maxWidth, maxHeight),
                            Mode = ResizeMode.Max // 等比例縮小
                        }));

                        using (var outputStream = new MemoryStream())
                        {
                            // 根據原始副檔名決定輸出格式
                            IImageEncoder encoder = extension switch
                            {
                                ".png" => new PngEncoder(),
                                ".jpg" or ".jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                                ".webp" => new SixLabors.ImageSharp.Formats.Webp.WebpEncoder(),
                                ".bmp" => new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(),
                                ".gif" => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                                ".ico" => new PngEncoder(), // ICO 轉成 PNG 輸出
                                _ => new PngEncoder()
                            };

                            await image.SaveAsync(outputStream, encoder);
                            return Convert.ToBase64String(outputStream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is UnknownImageFormatException || ex is InvalidImageContentException || ex is FormatException)
            {
                // Base64 字串無效或圖片格式不支援
                throw new InvalidOperationException($"無法處理圖片: {ex.Message}", ex);
            }
        }

        private static bool IsImage(string extension)
        {
            return Array.Exists(SupportedExtensions, e => e == extension);
        }

        private static bool IsUnsupportedImage(string extension)
        {
            return Array.Exists(UnsupportedByImageSharp, e => e == extension) || !Array.Exists(SupportedExtensions, e => e == extension);
        }

        private static string SanitizeBase64(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Base64 字串不能為空", nameof(input));
            }


            // 移除 base64 data URL 前綴（如果有）
            int index = input.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
            string cleanedInput = index >= 0 ? input[(index + 7)..] : input;

            // 移除空白字元
            cleanedInput = cleanedInput.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // 確保 Base64 字串長度正確（必須是 4 的倍數）
            int remainder = cleanedInput.Length % 4;
            if (remainder > 0)
            {
                cleanedInput += new string('=', 4 - remainder);
            }

            return cleanedInput;
        }

        private static bool IsValidImageHeader(byte[] imageBytes, out string extension)
        {
            extension = string.Empty;
            if (imageBytes.Length < 8)
            {
                return false;
            }


            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (imageBytes.Length >= 8 &&
                imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
            {
                extension = ".png";
                return true;
            }


            // JPEG: FF D8 FF
            if (imageBytes.Length >= 3 &&
                imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
            {
                extension = ".jpg";
                return true;
            }


            // GIF: GIF87a or GIF89a
            if (imageBytes.Length >= 6 &&
                imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
            {
                extension = ".gif";
                return true;
            }


            // BMP: BM
            if (imageBytes.Length >= 2 &&
                imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
            {
                extension = ".bmp";
                return true;
            }


            // WebP: RIFF....WEBP
            if (imageBytes.Length >= 12 &&
                imageBytes[0] == 0x52 && imageBytes[1] == 0x49 &&
                imageBytes[2] == 0x46 && imageBytes[3] == 0x46 &&
                imageBytes[8] == 0x57 && imageBytes[9] == 0x45 &&
                imageBytes[10] == 0x42 && imageBytes[11] == 0x50)
            {
                extension = ".webp";
                return true;
            }


            // ICO: 00 00 01 00 unsupport
            if (imageBytes.Length >= 4 &&
                imageBytes[0] == 0x00 && imageBytes[1] == 0x00 &&
                imageBytes[2] == 0x01 && imageBytes[3] == 0x00)
            {
                extension = ".ico";
                return true;
            }

            return false;
        }
    }
}
