import { Router } from "@angular/router";
import { Component, OnInit } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { environment } from "src/environments/environment";
import { AuthenticationService } from "./core/authentication/auth.service";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"],
})
export class AppComponent implements OnInit {
  constructor(
    private translateService: TranslateService,
  ) {}
  ngOnInit(): void {
    this.translateService.use(environment.defaultLanguage);
  }
}
