namespace F1Fantasy.Modules.AuthModule.Dtos
{
    public class ApplicationUserTokenResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public string TokenType { get; set; } = "JWT";

        public DateTime ExpiresAt { get; set; }
    }
}