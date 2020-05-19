import { EmployeeService } from "./../employee.service";
import { Component, OnInit } from "@angular/core";
import { ActionsService } from "./actions.service";
import { IUser } from "../profile/user.model";
import { DocumentType } from "./document-type.enum";
import { FormControl, Validators, FormGroup } from "@angular/forms";

@Component({
  selector: "app-actions",
  templateUrl: "./actions.component.html",
  styleUrls: ["./actions.component.css"],
})
export class ActionsComponent implements OnInit {
  public ducumentType = DocumentType;

  additionalUserForm: FormGroup = new FormGroup({
    Name: new FormControl("", Validators.required),
    Email: new FormControl("", [Validators.required, Validators.email]),
  });

  constructor(
    private actionServise: ActionsService,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {}

  sendEnvelope(type: DocumentType) {
    const user: IUser = this.employeeService.user;

    this.actionServise
      .sendEnvelop(type, user, "https://localhost:5001")
      .subscribe();
  }

  submit(type: DocumentType) {
    this.actionServise
      .sendEnvelop(
        type,
        this.additionalUserForm.value,
        "https://localhost:5001"
      )
      .subscribe();
  }

  createClickWrap(type: string) {
    this.actionServise
      .createClickWrap()
      .subscribe();
  }
  doAction() {}
}
