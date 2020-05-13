import { AuthType } from "./auth-type.enum";
import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { tap, catchError } from "rxjs/operators";
import { IUser } from "../employee/profile/user.model";
import { Router } from "@angular/router";
import { of } from "rxjs";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
  public isAuthenticated = false;
  user: IUser;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject("BASE_URL") private baseUrl: string
  ) {}

  login(authType: AuthType) {
    return this.http
      .get<any>(`account/login`, {
        params: {
          authType,
        },
      })
      .pipe(tap(() => (this.isAuthenticated = true)));
  }

  getUser() {
    return this.http.get<any>(this.baseUrl + "api/user").pipe(
      tap((result) => {
        this.user = result;
        this.isAuthenticated = true;
      })
    );
  }

  logout() {}
}
