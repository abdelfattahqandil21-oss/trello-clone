import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

const ERROR_MESSAGES: Record<number, string> = {
  400: 'Bad request',
  403: 'Forbidden',
  404: 'Not found',
  405: 'Method not allowed',
  408: 'Request timeout',
  409: 'Conflict',
  422: 'Invalid data',
  429: 'Too many requests',
  500: 'Internal server error',
  502: 'Bad gateway',
  503: 'Service unavailable',
  504: 'Gateway timeout',
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error) => {
      const status = error.status;

      if (status === 401) {
        notification.error('Session expired, please login again', 'Unauthorized');
        localStorage.removeItem('token');
        window.location.href = '/login';
        return throwError(() => error);
      }

      if (req.url.includes('/login')) {
        return throwError(() => error);
      }

      if (status === 0) {
        notification.error('Network error');
      } else {
        notification.error(ERROR_MESSAGES[status] || 'Unexpected error');
      }

      return throwError(() => error);
    })
  );
};
