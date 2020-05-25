import { Component } from "@angular/core";
import { AuthType } from "../core/authentication/auth-type.enum";
import { AuthenticationService } from "../core/authentication/auth.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})
export class HomeComponent {
  authType = AuthType;

  constructor(private authenticationService: AuthenticationService) {}

  ngOnInit(): void {}

  login(authType: AuthType) {
    this.authenticationService.saveAuthType(authType);
    window.location.href = `Account/Login?${authType}`;
  }
}
