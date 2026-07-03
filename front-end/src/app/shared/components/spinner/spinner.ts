import { Component, inject, input } from '@angular/core';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.html',
  styleUrl: './spinner.css'
})
export class Spinner {
  protected readonly loadingService = inject(LoadingService);
  readonly inline = input(false);
}
