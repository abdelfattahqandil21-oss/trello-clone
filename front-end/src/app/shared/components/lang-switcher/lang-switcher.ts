import { Component, inject } from '@angular/core';
import { TranslationService } from '../../../core/services/translation.service';
import { ViewTransitionService } from '../../../core/services/view-transition.service';

@Component({
  selector: 'app-lang-switcher',
  templateUrl: './lang-switcher.html',
  styleUrl: './lang-switcher.css'
})
export class LangSwitcher {
  protected readonly translation = inject(TranslationService);
  private readonly viewTransition = inject(ViewTransitionService);

  toggle(event: MouseEvent): void {
    const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
    this.viewTransition.start(() => this.translation.toggle(), rect.left + rect.width / 2, rect.top + rect.height / 2);
  }
}
