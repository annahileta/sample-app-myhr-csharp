import { Component } from "@angular/core";
import { AuthenticationService } from "./auth.service";
import { AuthType } from "./auth-type.enum";
import { Router } from "@angular/router";
import { Location } from "@angular/common";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})
export class HomeComponent {
  authType = AuthType;

  constructor(
    private location: Location,
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}
  ngOnInit(): void {
    if (this.authenticationService.isAuthenticated) {
      this.router.navigate(["/employee"]);
    }
  }
}
