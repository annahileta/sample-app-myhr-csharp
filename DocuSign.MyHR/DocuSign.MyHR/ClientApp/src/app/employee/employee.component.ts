import { EmployeeService } from "./employee.service";
import { Component, OnInit } from "@angular/core";
import { IUser } from "./models/user.model";

@Component({
  selector: "app-employee",
  templateUrl: "./employee.component.html",
  styleUrls: ["./employee.component.css"],
})
export class EmployeeComponent implements OnInit {
  constructor(private employeeService: EmployeeService) {}
  isEditUser = false;
  user: IUser;

  ngOnInit(): void {
    this.employeeService.getUser();
    this.employeeService.user$.subscribe((user) => (this.user = user));
  }

  editUser() {
    this.isEditUser = true;
  }

  cancelEdit() {
    this.isEditUser = false;
  }

  exitSaving(user: IUser) {
    this.user = user;
    this.isEditUser = false;
  }
}
