import { EmployeeService } from "./../employee.service";
import { Component, OnInit } from "@angular/core";
import { ActionsService } from "./actions.service";
import { IUser } from "../profile/user.model";
import { DocumentType } from "./document-type.enum";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { HttpResponse } from "@angular/common/http";

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
    this.actionServise
      .sendEnvelop(type, null, "https://localhost:5001")
      .subscribe((payload) => {
        window.location.href = payload.redirectUrl;
      });
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
    this.actionServise.createClickWrap().subscribe();
  }
  doAction() {}
}
