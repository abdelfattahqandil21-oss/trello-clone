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
  templateUrl: './register.html',
  styleUrl: './register.css',
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
