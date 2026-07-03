import { Service, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'transit-theme';

@Service()
export class ThemeService {
  
  readonly theme = signal<Theme>(this._getInitialTheme());

  constructor() {
    effect(() => {
      document.documentElement.setAttribute('data-theme', this.theme());
      localStorage.setItem(STORAGE_KEY, this.theme());
    });

    window.matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', (e) => {
        if (!localStorage.getItem(STORAGE_KEY)) {
          this.theme.set(e.matches ? 'dark' : 'light');
        }
      });
  }

  toggle(event?: MouseEvent): void {
    const newTheme = this.theme() === 'dark' ? 'light' : 'dark';

    if (!document.startViewTransition) {
      this.theme.set(newTheme);
      return;
    }

    const x = event?.clientX ?? window.innerWidth / 2;
    const y = event?.clientY ?? window.innerHeight / 2;
    const maxRadius = Math.hypot(
      Math.max(x, window.innerWidth - x),
      Math.max(y, window.innerHeight - y)
    );

    document.documentElement.style.setProperty('--click-x', `${x}px`);
    document.documentElement.style.setProperty('--click-y', `${y}px`);
    document.documentElement.style.setProperty('--max-radius', `${maxRadius}px`);

    document.startViewTransition(() => {
      this.theme.set(newTheme);
    });
  }

  isDark(): boolean {
    return this.theme() === 'dark';
  }

  private _getInitialTheme(): Theme {
    const stored = localStorage.getItem(STORAGE_KEY) as Theme | null;
    if (stored === 'light' || stored === 'dark') return stored;
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
  
}