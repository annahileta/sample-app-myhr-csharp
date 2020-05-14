import { AuthType } from "./auth-type.enum";
import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { tap } from "rxjs/operators";
import { IUser } from "../employee/profile/user.model";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
  user: IUser;
  public isAuthenticated = false;
  public authType: AuthType;

  constructor(
    private http: HttpClient,
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

  saveAuthType(authType: AuthType) {
    sessionStorage.setItem("authType", authType);
  }

  getAuthType() {
    return sessionStorage.getItem("authType");
  }
}
