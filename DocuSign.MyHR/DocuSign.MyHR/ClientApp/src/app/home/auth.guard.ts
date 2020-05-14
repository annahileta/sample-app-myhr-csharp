import { Injectable } from "@angular/core";

import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  CanLoad,
} from "@angular/router";
import { AuthenticationService } from "./auth.service";
import { HomeComponent } from "./home.component";

@Injectable({ providedIn: "root" })
export class AuthGuard implements CanLoad {
  constructor(
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}

  canLoad() {
    if (this.authenticationService.isAuthenticated) {
      return true;
    } else {
      this.router.navigateByUrl("/");
      return false;
    }
  }
}
