import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, map } from 'rxjs';
import { SearchRankPresentationRequestsLoginRequest } from '../../search-rank-api/models';
import { BearerTokenService } from '../../search-rank-api/services';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey: string = 'jwt-token';
  private readonly tokenService = inject(BearerTokenService);
  private readonly router = inject(Router);

  login(params: SearchRankPresentationRequestsLoginRequest): Observable<{ token: string | null }> {
    return this.tokenService.apiTokenPost$Response({ body: params }).pipe(
      map(response => ({ token: response.body.token ?? null })),
      tap(({ token }) => {
        if (token) {
          sessionStorage.setItem(this.tokenKey, token);
        }
      })
    );    
  }

  logout() {
    sessionStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
