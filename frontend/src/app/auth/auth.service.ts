import { AuthType } from './auth-type.enum';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  constructor(private http: HttpClient) {}

  login(authType: AuthType) {
    const callbackUrl = 'http://localhost:4200/ds/callback';
    const returnUrl = '';

    return this.http.get<any>(`${environment.apiUrl}/login`, {
      params: {
        authType,
        callbackUrl,
        returnUrl,
      },
    });
  }

  logout() {}
}
