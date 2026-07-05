import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { TokenStorageService } from './services/token-storage.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorageService);
  const router = inject(Router);
  const token = tokenStorage.getAccessToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err) => {
      if (err instanceof HttpErrorResponse && err.status === 401
          && !req.url.includes('/api/v1/auth/')) {
        tokenStorage.clearAccessToken();
        router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};
