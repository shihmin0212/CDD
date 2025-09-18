using FluentEmail.Core.Models;

namespace Sample.Api.Models.EmailService
{
    public class EmailMetaData
    {
        public IEnumerable<Address> ToAddress { get; set; }
        public IEnumerable<Address>? CCToAddress { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }
        public string? AttachmentPath { get; set; }

        public EmailMetaData(
            IEnumerable<Address> toAddress,
            IEnumerable<Address>? CCToAddress,
            string subject,
            string? body = null,
            string? attachmentPath = null)
        {
            ToAddress = toAddress;
            CCToAddress = CCToAddress;
            Subject = subject;
            Body = body;
            AttachmentPath = attachmentPath;
        }
    }
}
