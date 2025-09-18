namespace CDD.Web.Models.DTO
{
    public class ExternalSystem
    {
        public string SystemID { get; set; }

        public Guid ApiKey { get; set; }

        public string HashKey { get; set; }

        public string IVKey { get; set; }
    }
}
