using System.Net;

namespace CDD.Web.Models.APIResponse
{
    public interface IStatusCodeResp
    {
        HttpStatusCode ResponseStatusCode { get; set; }
    }
}
