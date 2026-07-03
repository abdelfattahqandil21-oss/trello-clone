import { signal, computed, effect, Service } from '@angular/core';

export type Lang = 'ar' | 'en';

const STORAGE_KEY = 'app_lang';

@Service()
export class TranslationService {
  private readonly lang = signal<Lang>(this.loadLang());

  readonly currentLang = this.lang.asReadonly();

  readonly dir = computed(() => (this.lang() === 'ar' ? 'rtl' : 'ltr'));

  readonly isRtl = computed(() => this.lang() === 'ar');

  constructor() {
    effect(() => {
      const l = this.lang();
      document.documentElement.lang = l;
      document.documentElement.dir = this.dir();
      localStorage.setItem(STORAGE_KEY, l);
    });
  }

  translate(ar: string, en: string): string {
    return this.lang() === 'ar' ? ar : en;
  }

  toggle(): void {
    this.lang.update(l => (l === 'ar' ? 'en' : 'ar'));
  }

  setLang(l: Lang): void {
    this.lang.set(l);
  }

  private loadLang(): Lang {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'en' || stored === 'ar' ? stored : 'en';
  }
}
