import { Injectable } from "@angular/core";

import { Router, CanLoad } from "@angular/router";
import { AuthenticationService } from "./auth.service";
import { mapTo, catchError } from "rxjs/operators";
import { of, Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class AuthGuard implements CanLoad {
  constructor(
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}

  canLoad(): Observable<boolean> | boolean {
    if (!this.authenticationService.isAuthenticated) {
      return this.authenticationService.getUser().pipe(
        mapTo(true),
        catchError(() => {
          this.router.navigate(["/"]);
          return of(false);
        })
      );
    }
    return true;
  }
}
