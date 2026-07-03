import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslationService } from '../core/services/translation.service';
import { LangSwitcher } from '../shared/components/lang-switcher/lang-switcher';
import { ThemeSwitcher } from '../shared/components/theme-switcher/theme-switcher';
import { Spinner } from '../shared/components/spinner/spinner';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, LangSwitcher, ThemeSwitcher, Spinner],
  templateUrl: './shell.html',
  styleUrl: './shell.css'
})
export class Shell {
  protected readonly translation = inject(TranslationService);
}
