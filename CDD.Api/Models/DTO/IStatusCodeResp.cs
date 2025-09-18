using System.Net;

namespace CDD.Api.Models.DTO
{
    public interface IStatusCodeResp
    {
        HttpStatusCode ResponseStatusCode { get; set; }
    }
}
