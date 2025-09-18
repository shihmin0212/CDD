using System.Net;

namespace Sample.Api.Models.DTO
{
    public interface IStatusCodeResp
    {
        HttpStatusCode ResponseStatusCode { get; set; }
    }
}
