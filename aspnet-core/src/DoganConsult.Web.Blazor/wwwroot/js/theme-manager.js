/**
 * DoganConsult Platform - Theme Manager
 * Handles theme switching, localStorage persistence, and dynamic updates
 */

class ThemeManager {
    constructor() {
        this.themes = ['light', 'dark', 'high-contrast'];
        this.languages = ['en', 'tr', 'de', 'es', 'fr'];
        this.init();
    }

    init() {
        this.loadSavedPreferences();
        this.setupEventListeners();
        this.updateSystemTheme();
    }

    loadSavedPreferences() {
        const savedTheme = localStorage.getItem('dogan-theme') || 'light';
        const savedLanguage = localStorage.getItem('dogan-language') || 'en';
        
        this.setTheme(savedTheme);
        this.setLanguage(savedLanguage);
    }

    setTheme(themeName) {
        if (!this.themes.includes(themeName)) {
            console.warn(`Unknown theme: ${themeName}`);
            return;
        }

        // Remove all theme classes
        this.themes.forEach(theme => {
            document.documentElement.classList.remove(`theme-${theme}`);
        });

        // Add the new theme class
        document.documentElement.classList.add(`theme-${themeName}`);
        
        // Update meta theme-color for mobile browsers
        const themeColorMap = {
            'light': '#ffffff',
            'dark': '#1a1a1a',
            'high-contrast': '#000000'
        };
        
        let metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (!metaThemeColor) {
            metaThemeColor = document.createElement('meta');
            metaThemeColor.name = 'theme-color';
            document.head.appendChild(metaThemeColor);
        }
        metaThemeColor.content = themeColorMap[themeName] || '#ffffff';

        // Save to localStorage
        localStorage.setItem('dogan-theme', themeName);
        
        // Dispatch custom event
        this.dispatchThemeChangeEvent(themeName);
        
        console.log(`Theme changed to: ${themeName}`);
    }

    setLanguage(languageCode) {
        if (!this.languages.includes(languageCode)) {
            console.warn(`Unknown language: ${languageCode}`);
            return;
        }

        // Update document language
        document.documentElement.lang = languageCode;
        
        // Save to localStorage
        localStorage.setItem('dogan-language', languageCode);
        
        // Dispatch custom event
        this.dispatchLanguageChangeEvent(languageCode);
        
        console.log(`Language changed to: ${languageCode}`);
    }

    getCurrentTheme() {
        return localStorage.getItem('dogan-theme') || 'light';
    }

    getCurrentLanguage() {
        return localStorage.getItem('dogan-language') || 'en';
    }

    toggleTheme() {
        const currentTheme = this.getCurrentTheme();
        const currentIndex = this.themes.indexOf(currentTheme);
        const nextIndex = (currentIndex + 1) % this.themes.length;
        const nextTheme = this.themes[nextIndex];
        
        this.setTheme(nextTheme);
        return nextTheme;
    }

    updateSystemTheme() {
        // Listen for system theme changes
        if (window.matchMedia) {
            const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
            const highContrastQuery = window.matchMedia('(prefers-contrast: high)');
            
            const handleSystemThemeChange = () => {
                const savedTheme = localStorage.getItem('dogan-theme');
                if (!savedTheme || savedTheme === 'auto') {
                    if (highContrastQuery.matches) {
                        this.setTheme('high-contrast');
                    } else if (darkModeQuery.matches) {
                        this.setTheme('dark');
                    } else {
                        this.setTheme('light');
                    }
                }
            };

            darkModeQuery.addEventListener('change', handleSystemThemeChange);
            highContrastQuery.addEventListener('change', handleSystemThemeChange);
        }
    }

    setupEventListeners() {
        // Listen for theme selector events
        document.addEventListener('click', (e) => {
            if (e.target.matches('.theme-option')) {
                const theme = e.target.dataset.theme;
                if (theme) {
                    this.setTheme(theme);
                }
            }
            
            if (e.target.matches('.language-option')) {
                const language = e.target.dataset.language;
                if (language) {
                    this.setLanguage(language);
                }
            }
        });

        // Listen for keyboard shortcuts
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + Shift + T = Toggle Theme
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.toggleTheme();
            }
        });
    }

    dispatchThemeChangeEvent(theme) {
        const event = new CustomEvent('themeChanged', {
            detail: { theme: theme },
            bubbles: true
        });
        document.dispatchEvent(event);
    }

    dispatchLanguageChangeEvent(language) {
        const event = new CustomEvent('languageChanged', {
            detail: { language: language },
            bubbles: true
        });
        document.dispatchEvent(event);
    }

    // Utility methods for components
    getThemeValue(variable) {
        return getComputedStyle(document.documentElement).getPropertyValue(variable);
    }

    setThemeValue(variable, value) {
        document.documentElement.style.setProperty(variable, value);
    }

    // Animation helpers
    animateThemeTransition() {
        document.documentElement.style.transition = 'background-color 0.3s ease, color 0.3s ease';
        setTimeout(() => {
            document.documentElement.style.transition = '';
        }, 300);
    }
}

// Initialize theme manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.DoganTheme = new ThemeManager();
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ThemeManager;
}