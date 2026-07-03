import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { form, FormField, FormRoot, required, email, minLength } from '@angular/forms/signals';
import { TranslationService } from '../../core/services/translation.service';
import { NotificationService } from '../../core/services/notification.service';
import { ThemeSwitcher } from '../../shared/components/theme-switcher/theme-switcher';
import { LangSwitcher } from '../../shared/components/lang-switcher/lang-switcher';

interface RegisterData {
  name: string;
  email: string;
  password: string;
}

@Component({
  selector: 'app-register',
  imports: [RouterLink, FormField, FormRoot, ThemeSwitcher, LangSwitcher],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-[var(--bg-page)] p-4 relative">
      <div class="absolute top-4 end-4 flex items-center gap-1 z-10">
        <app-theme-switcher />
        <app-lang-switcher />
      </div>
      <div class="w-full max-w-sm">
        <div class="text-center mb-8">
          <div class="w-14 h-14 bg-brand-primary rounded-2xl flex items-center justify-center mx-auto shadow-lg shadow-brand-primary/25">
            <i class="pi pi-th-large text-2xl text-white"></i>
          </div>
          <h1 class="text-2xl font-bold text-text-primary mt-4">TreClone</h1>
          <p class="text-text-secondary text-sm mt-1">{{ translation.translate('إنشاء حساب جديد', 'Create your account') }}</p>
        </div>

        <div class="bg-[var(--bg-card)] rounded-2xl shadow-[var(--shadow-card)] p-8">
          <form [formRoot]="f">
            <div class="space-y-5">
              <div>
                <label class="block text-sm font-medium text-text-secondary mb-1.5">{{ translation.translate('الاسم', 'Name') }}</label>
                <div class="relative">
                  <i class="pi pi-user absolute left-3 top-1/2 -translate-y-1/2 text-text-muted text-sm pointer-events-none"></i>
                  <input
                    [formField]="f.name"
                    type="text"
                    class="w-full pl-9 pr-3 py-2.5 rounded-xl border bg-surface text-text-primary placeholder:text-text-muted focus:outline-none focus:ring-2 transition-all text-sm outline-none"
                    [class.border-red-400]="f.name().touched() && f.name().invalid()"
                    [class.border-border]="!f.name().touched() || f.name().valid()"
                    [class.focus:ring-brand-primary/30]="!f.name().invalid()"
                    [class.focus:ring-red-300]="f.name().touched() && f.name().invalid()"
                    [class.focus:border-brand-primary]="!f.name().invalid()"
                    [class.focus:border-red-400]="f.name().touched() && f.name().invalid()"
                    [placeholder]="translation.translate('اسمك', 'Your name')"
                  />
                </div>
                @if (f.name().touched() && f.name().invalid()) {
                  @for (err of f.name().errors(); track err.kind) {
                    <p class="text-red-400 text-xs mt-1.5 flex items-center gap-1">
                      <i class="pi pi-exclamation-circle text-[10px]"></i>
                      {{ translation.translate('الاسم مطلوب', err.message ?? 'Name is required') }}
                    </p>
                  }
                }
              </div>

              <div>
                <label class="block text-sm font-medium text-text-secondary mb-1.5">{{ translation.translate('البريد الإلكتروني', 'Email') }}</label>
                <div class="relative">
                  <i class="pi pi-envelope absolute left-3 top-1/2 -translate-y-1/2 text-text-muted text-sm pointer-events-none"></i>
                  <input
                    [formField]="f.email"
                    type="email"
                    class="w-full pl-9 pr-3 py-2.5 rounded-xl border bg-surface text-text-primary placeholder:text-text-muted focus:outline-none focus:ring-2 transition-all text-sm outline-none"
                    [class.border-red-400]="f.email().touched() && f.email().invalid()"
                    [class.border-border]="!f.email().touched() || f.email().valid()"
                    [class.focus:ring-brand-primary/30]="!f.email().invalid()"
                    [class.focus:ring-red-300]="f.email().touched() && f.email().invalid()"
                    [class.focus:border-brand-primary]="!f.email().invalid()"
                    [class.focus:border-red-400]="f.email().touched() && f.email().invalid()"
                    placeholder="you@example.com"
                  />
                </div>
                @if (f.email().touched() && f.email().invalid()) {
                  @for (err of f.email().errors(); track err.kind) {
                    <p class="text-red-400 text-xs mt-1.5 flex items-center gap-1">
                      <i class="pi pi-exclamation-circle text-[10px]"></i>
                      {{ translation.translate('البريد الإلكتروني مطلوب', err.message ?? 'Email is required') }}
                    </p>
                  }
                }
              </div>

              <div>
                <label class="block text-sm font-medium text-text-secondary mb-1.5">{{ translation.translate('كلمة المرور', 'Password') }}</label>
                <div class="relative">
                  <i class="pi pi-lock absolute left-3 top-1/2 -translate-y-1/2 text-text-muted text-sm pointer-events-none"></i>
                  <input
                    [formField]="f.password"
                    type="password"
                    class="w-full pl-9 pr-3 py-2.5 rounded-xl border bg-surface text-text-primary placeholder:text-text-muted focus:outline-none focus:ring-2 transition-all text-sm outline-none"
                    [class.border-red-400]="f.password().touched() && f.password().invalid()"
                    [class.border-border]="!f.password().touched() || f.password().valid()"
                    [class.focus:ring-brand-primary/30]="!f.password().invalid()"
                    [class.focus:ring-red-300]="f.password().touched() && f.password().invalid()"
                    [class.focus:border-brand-primary]="!f.password().invalid()"
                    [class.focus:border-red-400]="f.password().touched() && f.password().invalid()"
                    placeholder="••••••••"
                  />
                </div>
                @if (f.password().touched() && f.password().invalid()) {
                  @for (err of f.password().errors(); track err.kind) {
                    <p class="text-red-400 text-xs mt-1.5 flex items-center gap-1">
                      <i class="pi pi-exclamation-circle text-[10px]"></i>
                      {{ translation.translate(err.kind === 'minLength' ? 'كلمة المرور أقصر من 6 أحرف' : 'كلمة المرور مطلوبة', err.message ?? 'Password is required') }}
                    </p>
                  }
                }
              </div>

              <button
                type="submit"
                [disabled]="f().submitting()"
                class="w-full py-2.5 bg-brand-primary text-white rounded-xl font-medium hover:bg-brand-accent transition-all text-sm cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                @if (f().submitting()) {
                  <i class="pi pi-spin pi-spinner"></i>
                  {{ translation.translate('جاري...', 'Creating account...') }}
                } @else {
                  {{ translation.translate('إنشاء حساب', 'Sign Up') }}
                }
              </button>
            </div>
          </form>

          <p class="text-center text-sm text-text-secondary mt-6">
            {{ translation.translate('لديك حساب بالفعل؟', 'Already have an account?') }}
            <a routerLink="/login" class="text-brand-primary font-medium hover:underline">{{ translation.translate('تسجيل دخول', 'Sign in') }}</a>
          </p>
        </div>
      </div>
    </div>
  `
})
export class Register {
  private readonly router = inject(Router);
  private readonly notification = inject(NotificationService);
  protected readonly translation = inject(TranslationService);

  readonly model = signal<RegisterData>({ name: '', email: '', password: '' });

  readonly f = form(this.model, (p) => {
    required(p.name, { message: 'Name is required' });
    required(p.email, { message: 'Email is required' });
    email(p.email, { message: 'Enter a valid email' });
    required(p.password, { message: 'Password is required' });
    minLength(p.password, 6, { message: 'Min 6 characters' });
  }, {
    submission: {
      action: async () => {
        this.notification.success(this.translation.translate('تم إنشاء الحساب بنجاح', 'Account created successfully'));
        this.router.navigate(['/boards']);
      },
      onInvalid: (field) => {
        const first = field().errorSummary()[0];
        first?.fieldTree().focusBoundControl();
        this.notification.error(this.translation.translate('يرجى تصحيح الأخطاء', 'Please fix the errors'));
      },
    },
  });
}
