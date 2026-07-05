import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { ApiService } from '../../core/services/api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';

export type AuthRequest = {
  usernameOrEmail: string;
  password: string;
};

type LoginResponse = {
  accessToken: string;
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(
    private readonly apiService: ApiService,
    private readonly tokenStorage: TokenStorageService
  ) {}

  register(request: AuthRequest): Observable<unknown> {
    return this.apiService.post('/api/v1/auth/register', {
      usernameOrEmail: request.usernameOrEmail,
      password: request.password
    });
  }

  login(request: AuthRequest): Observable<LoginResponse> {
    return this.apiService
      .post<LoginResponse>('/api/v1/auth/login', {
        usernameOrEmail: request.usernameOrEmail,
        password: request.password
      })
      .pipe(tap((response) => this.tokenStorage.setAccessToken(response.accessToken)));
  }
}
