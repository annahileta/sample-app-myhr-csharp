import { ToastService } from "./toast/toast.service";
import { EmployeeService } from "./employee.service";
import { Component, OnInit } from "@angular/core";
import { IUser } from "./models/user.model";
import { ActivatedRoute, Params } from "@angular/router";
import { filter, map } from "rxjs/operators";

@Component({
  selector: "app-employee",
  templateUrl: "./employee.component.html",
  styleUrls: ["./employee.component.css"],
})
export class EmployeeComponent implements OnInit {
  isEditUser = false;
  user: IUser;
  message: string;

  constructor(
    private employeeService: EmployeeService,
    private activatedRoute: ActivatedRoute,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.employeeService.getUser();
    this.employeeService.user$.subscribe((user) => (this.user = user));

    this.activatedRoute.queryParams
      .pipe(
        map((params: Params) => params["event"]),
        filter((event: string) => !!event)
      )
      .subscribe((event: string) => {
        if (event === "signing_complete") {         
          this.toastService.setToastMessage("Success");
        }
      });
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
