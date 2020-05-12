import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from './auth.service';
import { AuthType } from './auth-type.enum';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
})
export class AuthComponent implements OnInit {
  authType = AuthType;
  constructor(private authenticationService: AuthenticationService) {}
  ngOnInit(): void {}
  handleAuth(authType: AuthType) {
    this.authenticationService.login(authType).subscribe();
  }
}
