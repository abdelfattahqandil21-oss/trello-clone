import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { LoadingService, SKIP_GLOBAL_LOADING } from '../services/loading.service';

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);

  if (req.context.get(SKIP_GLOBAL_LOADING)) {
    return next(req);
  }

  loading.show();

  return next(req).pipe(finalize(() => loading.hide()));
};
