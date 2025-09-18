using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace CDD.Web.ControllerActionResults
{
    public class FileContentResultWithContentDisposition : FileContentResult
    {
        private const string ContentDispositionHeaderName = "Content-Disposition";

        public FileContentResultWithContentDisposition(byte[] fileContents, string contentType, ContentDisposition contentDisposition)
            : base(fileContents, contentType)
        {
            // check for null or invalid ctor arguments
            ContentDisposition = contentDisposition;
        }

        public ContentDisposition ContentDisposition { get; private set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            // check for null or invalid method argument
            ContentDisposition.FileName = ContentDisposition.FileName ?? FileDownloadName;
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            response.Headers.Add(ContentDispositionHeaderName, ContentDisposition.ToString());
            response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return base.ExecuteResultAsync(context);
        }
    }
}
