import { Component, OnInit } from "@angular/core";
import { IUser } from "./user.model";
import { EmployeeService } from "../employee.service";
import { FormGroup } from "@angular/forms";
import { Observable } from "rxjs";

@Component({
  selector: "app-profile",
  templateUrl: "./profile.component.html",
  styleUrls: ["./profile.component.css"],
})
export class ProfileComponent implements OnInit {
  user: IUser;

  constructor(private employeeService: EmployeeService) {}

  ngOnInit() { 
    this.employeeService.user$.subscribe((user) => (this.user = user));
  }

  saveUser(formValues) {
    this.employeeService.saveUser(formValues);
  }
  cancel() {
    this.user = { ...this.employeeService.user };
  }
}
