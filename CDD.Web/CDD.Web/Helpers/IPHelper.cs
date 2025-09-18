using System.Net;
using Microsoft.Extensions.Primitives;

namespace CDD.Web.Helpers
{
    public interface IIPHelper
    {
        string GetUserIP();

        string GetUserIPs();

        public bool IsIPMatchCIDR(string ip, string cidr);
    }

    public class IPHelper : IIPHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        private HttpContext httpContext
        {
            get
            {
                return contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(HttpContext));
            }
        }

        private readonly ILogger logger;

        public IPHelper(IHttpContextAccessor accessor, ILogger<IIPHelper> logger)
        {
            contextAccessor = accessor;
            this.logger = logger;
        }
        /// <summary>
        /// https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string GetUserIP()
        {
            string? ips = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For");
            string? ip = SplitCsv(ips)?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ip) && httpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");
            }
            return ip ?? String.Empty;
        }

        public string GetUserIPs()
        {
            string? _ips = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For");
            string? ips = _ips;
            if (string.IsNullOrWhiteSpace(_ips) && httpContext?.Connection?.RemoteIpAddress != null)
            {
                ips = httpContext.Connection.RemoteIpAddress.ToString();
            }
            if (string.IsNullOrWhiteSpace(ips))
            {
                ips = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");
            }
            return ips ?? String.Empty;
        }

        /// <summary>
        /// check if the ip matches CIDR.
        /// reference: https://stackoverflow.com/a/17210019/3000586
        /// </summary>
        public bool IsIPMatchCIDR(string ip, string cidr)
        {
            if (!cidr.Contains("/"))
            {
                return ip == cidr;
            }
            string[] parts = cidr.Split('/');
            if (parts[0] == "0.0.0.0")
            {
                return true;
            }
            int ipBytes = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int cidrAddrBytes = BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0);
            int cidrMaskBytes = IPAddress.HostToNetworkOrder(-1 << 32 - int.Parse(parts[1]));
            return (ipBytes & cidrMaskBytes) == (cidrAddrBytes & cidrMaskBytes);
        }

        private T? GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
        {
            StringValues values;
            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.
                if (!string.IsNullOrEmpty(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default;
        }

        private List<string>? SplitCsv(string? csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
            {
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();
            }

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

    }
}
