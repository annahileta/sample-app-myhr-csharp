import { EmployeeService } from "./../employee.service";
import { Component, OnInit } from "@angular/core";
import { ActionsService } from "./actions.service";
import { IUser } from "../profile/user.model";

@Component({
  selector: "app-actions",
  templateUrl: "./actions.component.html",
  styleUrls: ["./actions.component.css"],
})
export class ActionsComponent implements OnInit {
  constructor(
    private actionServise: ActionsService,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {}

  sendEnvelope(type: string) {
    const user: IUser = this.employeeService.user;
    this.actionServise
      .sendEnvelop(type, user, "https://localhost:5001")
      .subscribe();
  }
  createClickWrap(type: string) {
    this.actionServise
      .createClickWrap()
      .subscribe();
  }
  doAction() {}
}
