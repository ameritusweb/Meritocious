window.themeManager = {
    getInitialTheme: function () {
        const stored = localStorage.getItem('theme');
        if (stored) {
            return stored === 'dark';
        }
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    },

    setTheme: function (isDark) {
        document.documentElement.classList.toggle('dark', isDark);
        document.documentElement.style.colorScheme = isDark ? 'dark' : 'light';
        
        // Update Ant Design theme
        const theme = isDark ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme', theme);
        
        // Dispatch event for components to react
        window.dispatchEvent(new CustomEvent('themechange', { detail: { isDark } }));
    }
};