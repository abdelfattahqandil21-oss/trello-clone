import { inject, Service } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { type Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Service()
export class BaseApiService {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = environment.apiUrl;

  get<T>(endpoint: string, params?: Record<string, unknown>, context?: HttpContext): Observable<T> {
    let httpParams = new HttpParams();
    if (params) {
      for (const [key, value] of Object.entries(params)) {
        if (value != null && value !== '') {
          httpParams = httpParams.set(key, String(value));
        }
      }
    }
    return this.http.get<T>(`${this.baseUrl}${endpoint}`, { params: httpParams, context });
  }

  post<T>(endpoint: string, body?: unknown, context?: HttpContext): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${endpoint}`, body, { context });
  }

  put<T>(endpoint: string, body?: unknown, context?: HttpContext): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${endpoint}`, body, { context });
  }

  patch<T>(endpoint: string, body?: unknown, context?: HttpContext): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}${endpoint}`, body, { context });
  }

  delete<T>(endpoint: string, context?: HttpContext): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${endpoint}`, { context });
  }
}
