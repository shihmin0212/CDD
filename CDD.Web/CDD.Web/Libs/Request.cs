using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using CDD.Web.Models.DTO;

namespace CDD.Web.Libs
{
    public interface IStatusCodeResp
    {
        HttpStatusCode ResponseStatusCode { get; set; }
    }

    public interface IRequest
    {
        CancellationToken GetCancellationToken(int sec);
        string anonymizationLogBody(string input, string[]? noLogPatterns = null, string? replaceStr = null);

        Task<HttpStatusCode> Get(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns);
        Task<HttpStatusCode> Get(string url, Dictionary<string, string>? headers);
        Task<string> GetHtml(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogRespPatterns);
        Task<string> GetHtml(string url, Dictionary<string, string>? headers);
        Task<T> GetJSON<T>(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogResPatterns) where T : new();
        Task<T> GetJSON<T>(string url, Dictionary<string, string>? headers) where T : new();
        Task<HttpStatusCode> Post(string url, string postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns);
        Task<HttpStatusCode> Post(string url, string postBody, Dictionary<string, string>? headers);
        Task<T> PostJSON<T>(string url, string postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogResPatterns) where T : new();
        Task<T> PostJSON<T>(string url, string postBody, Dictionary<string, string>? headers) where T : new();
        Task<T> PostJSON<T>(string url, string postBody) where T : new();
        Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogResPatterns) where T : new();
        Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers) where T : new();
        Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody) where T : new();
        Task<T> GetXML<T>(string url, string encoding, string[] noLogReqPatterns, string[] noLogResPatterns) where T : new();
        Task<T> GetXML<T>(string url, string encoding) where T : new();
        Task<Byte[]> GetImage(string url);
        Task<Byte[]> GetFile(string url, string mediaType);
    }

    public class Request : IRequest
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Request Timeout sec 
        /// </summary>
        private int timeout;

        public CancellationToken GetCancellationToken(int sec)
        {
            return new CancellationTokenSource(sec * 1000).Token;
        }

        private readonly IHttpContextAccessor _contextAccessor;

        private HttpContext? httpContext
        {
            get
            {
                return _contextAccessor.HttpContext;
            }
        }

        private readonly HttpClient _client;

        private Guid ActionId;

        public Request(IHttpContextAccessor contextAccessor, ILogger<Request> logger, IConfiguration config, HttpClient client)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
            ConfigurationSection webSetting = (ConfigurationSection)config.GetSection("WebSetting");
            _client = client;
            timeout = webSetting.GetValue("ApiRequestTimeoutSeconds", 120);

            // For Logger
            string? itemsId = httpContext?.Items[HttpContextItemKey.ActionId]?.ToString() ?? Guid.NewGuid().ToString();
            this.ActionId = new Guid(itemsId);
        }

        /// <summary>
        /// 匿名化 logBody
        /// </summary>
        /// <param name="input"></param>
        /// <param name="noLogPatterns"></param>
        /// <returns></returns>
        public string anonymizationLogBody(string input, string[]? noLogPatterns = null, string? replaceStr = null)
        {
            string output = input;
            noLogPatterns = noLogPatterns ?? new string[] { };
            foreach (string regex in noLogPatterns)
            {
                output = Regex.Replace(output, regex, (String.IsNullOrWhiteSpace(replaceStr)) ? regex : replaceStr);
            }
            return output;
        }

        /// <summary>
        /// 取HttpStatusCode
        /// </summary>
        /// <param name="url"></param>
        /// <param name="noLogReqPatterns"></param>
        /// <param name="noLogRespPatterns"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Get(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetJson";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = anonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            // Add custom headers
            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    // request.Headers.Add("Authorization", "Bearer your-token");
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            using var response = await _client.SendAsync(request, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            #region Log Response 
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> Get(string url, Dictionary<string, string>? headers)
        {
            return await Get(url, headers, new string[] { });
        }


        /// <summary>
        /// 讀取text/html;
        /// </summary>
        /// <param name="url"></param>
        /// <param name="noLogReqPatterns"></param>
        /// <param name="noLogRespPatterns"></param>
        /// <returns></returns>
        public async Task<string> GetHtml(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogRespPatterns)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetJson";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = anonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            // Add custom headers
            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    // request.Headers.Add("Authorization", "Bearer your-token");
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            string result;
            using var response = await _client.SendAsync(request, GetCancellationToken(this.timeout));


            logBody.ResponseStatusCode = (int)response.StatusCode;
            // charset=big5
            var contentType = response.Content.Headers.ContentType;
            if (string.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response 
            logBody.ResponseData = anonymizationLogBody(Regex.Replace(result, @"\t|\n|\r|\s", String.Empty), noLogRespPatterns);
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return result;
        }
        public async Task<string> GetHtml(string url, Dictionary<string, string>? headers)
        {
            return await GetHtml(url, headers, new string[] { }, new string[] { });
        }

        /// <summary>
        /// Call API GET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="noLogReqPatterns"></param>
        /// <param name="noLogResPatterns"></param>
        /// <returns></returns>
        public async Task<T> GetJSON<T>(string url, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogRespPatterns) where T : new()
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetJson";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = anonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            // Add custom headers
            if (headers != null && headers.Any())
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    // request.Headers.Add("Authorization", "Bearer your-token");
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            string result;
            using var response = await _client.SendAsync(request, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            var contentType = response.Content.Headers.ContentType;
            if (string.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response 
            logBody.ResponseData = anonymizationLogBody(Regex.Replace(result, @"\t|\n|\r|\s", String.Empty), noLogRespPatterns);
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(string))
            {
                // no need to parse json (may contains special characters and throws error)
                return (T)(object)result;
            }
            else
            {
                T resultObj = JsonConvert.DeserializeObject<T>(result) ?? new T();

                if (typeof(IStatusCodeResp).IsAssignableFrom(typeof(T)))
                {
                    ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                }
                return resultObj;
            }
        }
        public async Task<T> GetJSON<T>(string url, Dictionary<string, string>? headers) where T : new()
        {
            return await GetJSON<T>(url, headers, new string[] { }, new string[] { });
        }


        public async Task<HttpStatusCode> Post(string url, string postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "PostJSON";
            logBody.Method = HttpMethod.Post.ToString();
            logBody.Url = url;
            logBody.RequestHeaders = headers;
            logBody.RequestModel = this.anonymizationLogBody(postBody, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            StringContent content = new StringContent(postBody, Encoding.UTF8, "application/json");
            // Add Spec headers
            #region Add Headers
            if (headers?.Count() > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }
            }
            #endregion

            using var response = await _client.PostAsync(url, content, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            #region Log Response 
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> Post(string url, string postBody, Dictionary<string, string>? headers = null)
        {
            return await Post(url, postBody, headers, new string[] { });
        }

        public async Task<T> PostJSON<T>(string url, string postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogRespPatterns) where T : new()
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "PostJSON";
            logBody.Method = HttpMethod.Post.ToString();
            logBody.Url = url;
            logBody.RequestHeaders = headers;
            logBody.RequestModel = this.anonymizationLogBody(postBody, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            string result;
            StringContent content = new StringContent(postBody, Encoding.UTF8, "application/json");
            // Add Spec headers
            #region Add Headers
            if (headers?.Count() > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }
            }
            #endregion

            using var response = await _client.PostAsync(url, content, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;
            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            if (string.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response 

            string logResult = this.anonymizationLogBody(Regex.Replace(result, @"\t|\n|\r|\s", String.Empty), noLogRespPatterns);
            logBody.LastUpdate = DateTime.Now;
            logBody.ResponseData = logResult;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(string))
            {
                // no need to parse json (may contains special characters and throws error)
                return (T)(object)result;
            }
            else
            {
                T resultObj = JsonConvert.DeserializeObject<T>(result) ?? new T();

                if (typeof(IStatusCodeResp).IsAssignableFrom(typeof(T)))
                {
                    ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                }
                return resultObj;
            }
        }
        public async Task<T> PostJSON<T>(string url, string postBody, Dictionary<string, string>? headers = null) where T : new()
        {
            return await PostJSON<T>(url, postBody, headers, new string[] { }, new string[] { });
        }
        public async Task<T> PostJSON<T>(string url, string postBody) where T : new()
        {
            return await PostJSON<T>(url, postBody, null, new string[] { }, new string[] { });
        }

        /// <summary>
        ///  x-www-form-urlencoded POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postBody"></param>
        /// <param name="noLogReqPatterns"></param>
        /// <param name="noLogResPatterns"></param>
        /// <returns></returns>
        public async Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers, string[] noLogReqPatterns, string[] noLogRespPatterns) where T : new()
        {
            string result;
            // Add Spec headers
            #region Add Headers
            if (headers?.Count() > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    postBody.Headers.Add(header.Key, header.Value);
                }
            }
            #endregion

            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "PostForm";
            logBody.Method = HttpMethod.Post.ToString();
            logBody.Url = url;
            logBody.RequestHeaders = headers;
            string postBodyString = await postBody.ReadAsStringAsync();
            logBody.RequestModel = this.anonymizationLogBody(postBodyString, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            using var response = await _client.PostAsync(url, postBody, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            var contentType = response.Content.Headers.ContentType;
            if (string.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response 
            logBody.ResponseStatusCode = (int)response.StatusCode;
            string logResult = this.anonymizationLogBody(Regex.Replace(result, @"\t|\n|\r|\s", String.Empty), noLogRespPatterns);
            logBody.ResponseData = logResult;
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(string))
            {
                // no need to parse json (may contains special characters and throws error)
                return (T)(object)result;
            }
            else
            {
                T resultObj = JsonConvert.DeserializeObject<T>(result) ?? new T();

                if (typeof(IStatusCodeResp).IsAssignableFrom(typeof(T)))
                {
                    ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                }
                return resultObj;
            }
        }
        public async Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers = null) where T : new()
        {
            return await PostForm<T>(url, postBody, headers, new string[] { }, new string[] { });
        }
        public async Task<T> PostForm<T>(string url, FormUrlEncodedContent postBody) where T : new()
        {
            return await PostForm<T>(url, postBody, null, new string[] { }, new string[] { });
        }

        public async Task<T> GetXML<T>(string url, string encoding, string[] noLogReqPatterns, string[] noLogRespPatterns) where T : new()
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetXML";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = anonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            using var response = await _client.GetAsync(url, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            using Stream result = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(result, Encoding.GetEncoding(encoding));
            var byteArray = await response.Content.ReadAsByteArrayAsync();
            var resString = Encoding.GetEncoding(encoding).GetString(byteArray, 0, byteArray.Length);

            #region Log Response
            string logResult = this.anonymizationLogBody(Regex.Replace(resString, @"\t|\n|\r|\s", String.Empty), noLogRespPatterns);
            logBody.ResponseData = logResult;
            this.LogMsg(logBody);
            #endregion
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T resultObj = (T)(serializer.Deserialize(result) ?? new T());
            if (typeof(IStatusCodeResp).IsAssignableFrom(typeof(T)))
            {
                ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
            }
            return resultObj;
        }
        public async Task<T> GetXML<T>(string url, string encoding) where T : new()
        {
            return await GetXML<T>(url, encoding, new string[] { }, new string[] { });
        }

        public async Task<byte[]> GetImage(string url)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetImage";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = url;
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            using var response = await _client.GetAsync(url, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType ?? null;
            if (contentType == null || !(contentType.MediaType != null && contentType.MediaType.StartsWith("image")))
            {
                return null;
            }
            byte[] byteArray = await response.Content.ReadAsByteArrayAsync();

            #region Log Response 
            logBody.ResponseStatusCode = (int)response.StatusCode;
            string logResult = $"resLength={byteArray.Length}";
            logBody.ResponseData = logResult;
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return byteArray;
        }

        public async Task<byte[]> GetFile(string url, string mediaType)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetFile";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = url;
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            using var response = await _client.GetAsync(url, GetCancellationToken(this.timeout));
            logBody.ResponseStatusCode = (int)response.StatusCode;

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType ?? null;
            if (contentType == null || !(contentType.MediaType != null && contentType.MediaType.StartsWith(mediaType)))
            {
                return null;
            }
            byte[] byteArray = await response.Content.ReadAsByteArrayAsync();

            #region Log Response 
            logBody.ResponseStatusCode = (int)response.StatusCode;
            string logResult = $"resLength={byteArray.Length}";
            logBody.ResponseData = logResult;
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return byteArray;
        }


        private void LogMsg(ApiLog logBody)
        {
            //  pass HttpContext.Items value for Nlog
            //Reset Id For Nlog write DB
            if (httpContext != null && httpContext.Items != null)
            {
                httpContext.Items[HttpContextItemKey.CallApiActionId] = logBody.CallApiActionId;
            }
            _logger.LogInformation(JsonConvert.SerializeObject(logBody));
        }

    }

}
