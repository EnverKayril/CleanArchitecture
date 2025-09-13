namespace CleanArchitecture.Infrastructure.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FromName { get; set; } = "No-Reply";
        public string FromAddress { get; set; } = default!;
        public bool UseStartTls { get; set; } = true;
    }
}
