import { Component, OnInit } from '@angular/core'
import { AuthenticationService } from '../authentication/auth.service'
import { Router, NavigationEnd } from '@angular/router'
import { filter } from 'rxjs/operators'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
  private isHomePage: boolean;

  constructor (private authenticationService: AuthenticationService, private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(res => {
      if (this.router.url === '/employee') {
        this.isHomePage = false
      } else {
        this.isHomePage = true
      }
    })
  }

  ngOnInit (): void {}

  logout (): void{
    this.authenticationService.logout()
  }
}
