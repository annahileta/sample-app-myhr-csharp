import { Injectable } from "@angular/core";
import { Router, CanLoad, Route, UrlSegment } from "@angular/router";
import { AuthenticationService } from "./auth.service";
import { catchError, map } from "rxjs/operators";
import { of, Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class AuthGuard implements CanLoad {
  constructor(
    private authenticationService: AuthenticationService,
    private router: Router
  ) {
  }

  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> | boolean {
    if (segments.length == 0) {
      return this.authenticationService.isAuthenticated().pipe(
        map((res: boolean) => {
          if (res) {
            this.router.navigate(["/employee"]);
            return false;
          } else {
            return true;
          }
        }),
        catchError(() => {
          return of(true);
        })
      );
    }
    if (segments[0].path == "employee") {
      return this.authenticationService.isAuthenticated().pipe(
        map((res: boolean) => {
          if (!res) {
            this.router.navigate(["/"]);
            return false;
          } else {
            return true;
          }
        }),
        catchError(() => {
          return of(true);
        })
      );
    }
  }
}
