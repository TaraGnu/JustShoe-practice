using JustShop.Commands;
using JustShop.Models;
using Microsoft.EntityFrameworkCore;

namespace JustShop.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly UserSession _session;
        private string _loginInput = string.Empty;
        private string _errorMessage = string.Empty;

        public LoginViewModel(UserSession session) => _session = session;

        public string LoginInput
        {
            get => _loginInput;
            set
            {
                if (SetProperty(ref _loginInput, value))
                    ErrorMessage = string.Empty;
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool Login(string password)
        {
            try
            {
                var user = Authenticate(LoginInput, password);
                _session.SignIn(user);
                return true;
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Не удалось войти. Техническая информация: {ex.Message}";
            }
            return false;
        }

        private static User Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Введите логин — пустое поле недопустимо.", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Введите пароль — пустое поле недопустимо.", nameof(password));

            var normalizedLogin = login.Trim();

            using var context = new ShoesShopContext();
            var user = context.Users
                .AsNoTracking()
                .Include(u => u.IdRoleNavigation)
                .FirstOrDefault(u => u.Login == normalizedLogin);

            if (user is null)
                throw new InvalidOperationException(
                    $"Пользователь с логином «{normalizedLogin}» не найден. Проверьте написание.");

            if (user.Password != password)
                throw new InvalidOperationException(
                    "Неверный пароль. Проверьте раскладку клавиатуры и Caps Lock.");

            return user;
        }
    }
}
