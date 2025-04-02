using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Meritocious.Blazor.Services.Theme
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        Task ToggleThemeAsync();
        Task InitializeAsync();
    }

    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isDarkMode;

        public bool IsDarkMode => _isDarkMode;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            // Check system preference and stored preference
            var prefersDark = await _jsRuntime.InvokeAsync<bool>("themeManager.getInitialTheme");
            _isDarkMode = prefersDark;
            await ApplyThemeAsync();
        }

        public async Task ToggleThemeAsync()
        {
            _isDarkMode = !_isDarkMode;
            await ApplyThemeAsync();
        }

        private async Task ApplyThemeAsync()
        {
            await _jsRuntime.InvokeVoidAsync("themeManager.setTheme", _isDarkMode);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", _isDarkMode ? "dark" : "light");
        }
    }
}