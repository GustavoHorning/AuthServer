using Microsoft.Extensions.Configuration;

namespace AuthServer.Security
{
    public class SecuritySettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public int ExpireHours { get; set; }
    }
}
