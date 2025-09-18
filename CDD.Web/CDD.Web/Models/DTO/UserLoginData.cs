namespace CDD.Web.Models.DTO
{
    public class UserLoginData
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? LoginTime { get; set; }

        public string? IPAddress { get; set; }
    }
}
