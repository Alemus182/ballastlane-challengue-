import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

type HeaderValue = string | string[];

type RequestOptions = {
  headers?: HttpHeaders | Record<string, HeaderValue>;
  params?: HttpParams | Record<string, string | number | boolean | ReadonlyArray<string | number | boolean>>;
};

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly apiBaseUrl = environment.apiBaseUrl.replace(/\/$/, '');

  constructor(private readonly http: HttpClient) {}

  get<T>(endpoint: string, options?: RequestOptions): Observable<T> {
    return this.http.get<T>(this.buildUrl(endpoint), options);
  }

  post<T>(endpoint: string, body: unknown, options?: RequestOptions): Observable<T> {
    return this.http.post<T>(this.buildUrl(endpoint), body, options);
  }

  put<T>(endpoint: string, body: unknown, options?: RequestOptions): Observable<T> {
    return this.http.put<T>(this.buildUrl(endpoint), body, options);
  }

  delete<T>(endpoint: string, options?: RequestOptions): Observable<T> {
    return this.http.delete<T>(this.buildUrl(endpoint), options);
  }

  private buildUrl(endpoint: string): string {
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
    return `${this.apiBaseUrl}${normalizedEndpoint}`;
  }
}
