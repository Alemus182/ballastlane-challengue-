import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  private readonly accessTokenKey = 'btr_access_token';

  // Session storage is used for MVP to avoid persisting tokens beyond the browser session.
  getAccessToken(): string | null {
    return sessionStorage.getItem(this.accessTokenKey);
  }

  setAccessToken(token: string): void {
    sessionStorage.setItem(this.accessTokenKey, token);
  }

  clearAccessToken(): void {
    sessionStorage.removeItem(this.accessTokenKey);
  }
}
