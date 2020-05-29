import { Observable } from "rxjs";
import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../authentication/auth.service";
import { Router, NavigationEnd } from "@angular/router";
import { map, filter, startWith } from "rxjs/operators";

@Component({
  selector: "app-header",
  templateUrl: "./header.component.html",
  styleUrls: ["./header.component.css"],
})
export class HeaderComponent implements OnInit {
  isHomePage$: Observable<boolean>;

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isHomePage$ = this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map((event: NavigationEnd) => event.url === "/"),
      startWith(true)
    );
  }

  logout() {
    this.authenticationService.logout();
  }
}
