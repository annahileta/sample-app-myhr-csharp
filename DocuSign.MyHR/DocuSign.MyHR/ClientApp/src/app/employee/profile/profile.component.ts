import { Component, OnInit, Input } from "@angular/core";
import { EmployeeService } from "../employee.service";
import { FormGroup } from "@angular/forms";
import { Observable } from "rxjs";
import { IUser } from "../models/user.model";
import { Output, EventEmitter } from '@angular/core'; 

@Component({
  selector: "app-profile",
  templateUrl: "./profile.component.html",
  styleUrls: ["./profile.component.css"],
})
export class ProfileComponent implements OnInit {
  @Output() editUserClicked = new EventEmitter<void>();
  @Input() user: IUser;

  constructor(private employeeService: EmployeeService) { }

  ngOnInit() {
  }

  editProfile() {
    this.editUserClicked.next();
  }

  saveUser(formValues) {
    this.employeeService.saveUser(formValues);
  }
  cancel() {
    this.user = { ...this.employeeService.user };
  }
}
