import { signal, Service } from '@angular/core';
import { HttpContextToken } from '@angular/common/http';

export const SKIP_GLOBAL_LOADING = new HttpContextToken<boolean>(() => false);

@Service()
export class LoadingService {
  readonly isLoading = signal(false);

  private pendingRequests = 0;

  show(): void {
    this.pendingRequests++;
    this.isLoading.set(true);
  }

  hide(): void {
    this.pendingRequests = Math.max(0, this.pendingRequests - 1);
    if (this.pendingRequests === 0) {
      this.isLoading.set(false);
    }
  }

  reset(): void {
    this.pendingRequests = 0;
    this.isLoading.set(false);
  }
}
