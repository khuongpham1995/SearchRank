import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    private readonly document = inject(DOCUMENT);
    private readonly currentTheme = signal<Theme>('light');
    private static readonly darkMode: string = 'dark-mode';
    private static readonly localStorageKey: string = 'current-theme';

    constructor() {
        this.setTheme(this.getThemeFromLocalStorage());
    }
    
    private setTheme(theme: Theme) {
        this.currentTheme.set(theme);
        if (theme === 'dark') {
            this.document.documentElement.classList.add(ThemeService.darkMode);
        }
        else {
            this.document.documentElement.classList.remove(ThemeService.darkMode);
        }

        this.setThemeInLocalStorage(theme);
    }

    toggleTheme() {
        if (this.currentTheme() === 'dark') {
            this.setTheme('light');
        } else {
            this.setTheme('dark');
        }
    }

    setThemeInLocalStorage(theme: Theme) {
        localStorage.setItem(ThemeService.localStorageKey, theme);
    }

    getThemeFromLocalStorage(): Theme {
        return localStorage.getItem(ThemeService.localStorageKey) as Theme || this.currentTheme();
    }
}