using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Meritocious.Blazor.Services.Forms
{
    public interface IValidationService
    {
        Task<bool> ValidatePasswordStrengthAsync(string password);
        Task<bool> ValidateUsernameAvailabilityAsync(string username);
        bool ValidateEmail(string email);
    }

    public class ValidationService : IValidationService
    {
        private readonly IUserService _userService;
        
        public ValidationService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> ValidatePasswordStrengthAsync(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar;
        }

        public async Task<bool> ValidateUsernameAvailabilityAsync(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 3)
                return false;

            return await _userService.CheckUsernameAvailabilityAsync(username);
        }

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}