namespace F1Fantasy.Core.Configurations
{
    public class AuthConfiguration
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public int TokenValidityInHours { get; set; } = 1;

        public int RefreshTokenValidityInDays { get; set; } = 7;
    }
}