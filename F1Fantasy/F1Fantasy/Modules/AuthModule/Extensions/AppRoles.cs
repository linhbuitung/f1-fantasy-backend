namespace F1Fantasy.Modules.AuthModule.Extensions
{
    public static class AppRoles
    {
        public const string Player = "Player";
        public const string NormalizedPlayer = "PLAYER";

        public const string Admin = "Admin";
        public const string NormalizedAdmin = "ADMIN";
        public const string SuperAdmin = "SuperAdmin";
        public const string NormalizedSuperAdmin = "SUPERADMIN";

        public static readonly string[] All = { Player, Admin, SuperAdmin };
    }
}