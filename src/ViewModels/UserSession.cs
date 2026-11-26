using JustShop.Models;

namespace JustShop.ViewModels
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser is not null;
        public string? Role => CurrentUser?.IdRoleNavigation?.Name;

        public bool IsAdmin => MatchesRole("Администратор", "Administrator", "Admin");
        public bool IsManager => MatchesRole("Менеджер", "Manager");
        public bool IsClient => MatchesRole("Клиент", "Client", "Customer");
        public bool IsStaff => IsAdmin || IsManager;

        public void SignIn(User user) => CurrentUser = user;
        public void SignOut() => CurrentUser = null;

        private bool MatchesRole(params string[] names)
        {
            var role = Role;
            if (string.IsNullOrWhiteSpace(role))
                return false;

            var trimmed = role.Trim();
            return names.Any(n => string.Equals(n, trimmed, StringComparison.OrdinalIgnoreCase));
        }
    }
}
