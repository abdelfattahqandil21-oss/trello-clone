import { Service } from '@angular/core';

@Service()
export class NotificationService {
  success(message: string, title?: string): void {
    this.showToast(message, 'success', title);
  }

  error(message: string, title?: string): void {
    this.showToast(message, 'error', title);
  }

  warning(message: string, title?: string): void {
    this.showToast(message, 'warning', title);
  }

  info(message: string, title?: string): void {
    this.showToast(message, 'info', title);
  }

  private showToast(message: string, type: 'success' | 'error' | 'warning' | 'info', title?: string): void {
    this.ensureStyles();

    const container = this.getContainer();
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;

    const icons: Record<string, string> = {
      success: '<i class="pi pi-check-circle"></i>',
      error: '<i class="pi pi-times-circle"></i>',
      warning: '<i class="pi pi-exclamation-triangle"></i>',
      info: '<i class="pi pi-info-circle"></i>',
    };

    toast.innerHTML = `
      <div class="toast-icon">${icons[type]}</div>
      <div class="toast-content">
        <div class="toast-title">${title || this.getDefaultTitle(type)}</div>
        <div class="toast-message">${message}</div>
      </div>
      <button class="toast-close">&times;</button>
    `;

    toast.querySelector('.toast-close')?.addEventListener('click', () => this.removeToast(toast));
    container.appendChild(toast);

    requestAnimationFrame(() => toast.classList.add('toast-visible'));

    setTimeout(() => this.removeToast(toast), 4000);
  }

  private getContainer(): HTMLElement {
    let container = document.getElementById('toast-container');
    if (!container) {
      container = document.createElement('div');
      container.id = 'toast-container';
      container.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        display: flex;
        flex-direction: column;
        gap: 10px;
        pointer-events: none;
      `;
      document.body.appendChild(container);
    }
    return container;
  }

  private removeToast(toast: HTMLElement): void {
    toast.classList.remove('toast-visible');
    toast.classList.add('toast-hidden');
    setTimeout(() => toast.remove(), 300);
  }

  private getDefaultTitle(type: string): string {
    const titles: Record<string, string> = {
      success: 'Success',
      error: 'Error',
      warning: 'Warning',
      info: 'Info',
    };
    return titles[type] || '';
  }

  private ensureStyles(): void {
    if (document.getElementById('toast-brand-styles')) return;

    const style = document.createElement('style');
    style.id = 'toast-brand-styles';
    style.textContent = `
      .toast {
        display: flex;
        align-items: flex-start;
        gap: 12px;
        background: #fff;
        border-radius: 12px;
        padding: 14px 18px;
        min-width: 320px;
        max-width: 420px;
        box-shadow: 0 8px 30px rgba(0,0,0,.12);
        pointer-events: auto;
        transform: translateX(120%);
        opacity: 0;
        transition: transform 0.35s cubic-bezier(.2,.9,.4,1.1), opacity 0.3s;
        border-right: 4px solid transparent;
      }
      .toast-visible {
        transform: translateX(0);
        opacity: 1;
      }
      .toast-hidden {
        transform: translateX(120%);
        opacity: 0;
      }
      .toast-icon {
        flex-shrink: 0;
        width: 36px;
        height: 36px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 50%;
        font-size: 18px;
        color: #fff;
      }
      .toast-success .toast-icon { background: #22C55E; }
      .toast-error .toast-icon { background: #EF4444; }
      .toast-warning .toast-icon { background: #F59E0B; }
      .toast-info .toast-icon { background: #3B82F6; }

      .toast-success { border-right-color: #22C55E; }
      .toast-error { border-right-color: #EF4444; }
      .toast-warning { border-right-color: #F59E0B; }
      .toast-info { border-right-color: #3B82F6; }

      .toast-content { flex: 1; min-width: 0; }
      .toast-title {
        font-size: 15px;
        font-weight: 700;
        color: #1F2937;
        margin-bottom: 2px;
      }
      .toast-message {
        font-size: 13px;
        color: #4b5563;
        line-height: 1.5;
      }
      .toast-close {
        flex-shrink: 0;
        background: none;
        border: none;
        font-size: 20px;
        color: #9ca3af;
        cursor: pointer;
        padding: 0 2px;
        line-height: 1;
        transition: color 0.15s;
      }
      .toast-close:hover { color: #374151; }
    `;
    document.head.appendChild(style);
  }
}
