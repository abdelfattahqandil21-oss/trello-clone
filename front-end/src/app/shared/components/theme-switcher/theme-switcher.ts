import { Component, inject } from '@angular/core';
import { ThemeService } from '../../../core/services/theme.service';
import { ViewTransitionService } from '../../../core/services/view-transition.service';

@Component({
  selector: 'app-theme-switcher',
  templateUrl: './theme-switcher.html',
  styleUrl: './theme-switcher.css'
})
export class ThemeSwitcher {
  protected readonly themeService = inject(ThemeService);
  private readonly viewTransition = inject(ViewTransitionService);

toggle(event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement | null;

  if (!target) {
    this.themeService.toggle();
    return;
  }

  const rect = target.getBoundingClientRect();

  this.viewTransition.start(
    () => this.themeService.toggle(),
    rect.left + rect.width / 2,
    rect.top + rect.height / 2
  );
}
}