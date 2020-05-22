import { AuthType } from "./auth-type.enum";
import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { tap, map } from "rxjs/operators";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
  private isUserAuthenticated: boolean | null;
  public authType: AuthType;

  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string
  ) { }

  isAuthenticated() {
    return this.http.get<boolean>(this.baseUrl + "api/isauthenticated").pipe(
      tap((result: boolean) => {
        this.isUserAuthenticated = result;
      }));
  }

  saveAuthType(authType: AuthType) {
    sessionStorage.setItem("authType", authType);
  }

  getAuthType() {
    return sessionStorage.getItem("authType");
  }

  logout() {
    sessionStorage.removeItem("authType");
    this.isUserAuthenticated = false;
  }
}
