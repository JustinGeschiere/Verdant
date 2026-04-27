namespace Domain.Roles
{
    public static class NamedRoles
    {
        public const string System = "System";

        public static IEnumerable<string> All => [System];
    }
}
