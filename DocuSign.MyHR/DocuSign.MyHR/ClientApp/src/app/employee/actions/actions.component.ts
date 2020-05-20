import { Component, OnInit } from "@angular/core";
import { ActionsService } from "./actions.service";
import { DocumentType } from "./document-type.enum";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";

@Component({
  selector: "app-actions",
  templateUrl: "./actions.component.html",
  styleUrls: ["./actions.component.css"],
})
export class ActionsComponent implements OnInit {
  public documentType = DocumentType;
  private type: DocumentType;
  private redirectUrl: string = window.location.href;

  additionalUserForm: FormGroup = new FormGroup({
    Name: new FormControl("", Validators.required),
    Email: new FormControl("", [Validators.required, Validators.email]),
  });

  constructor(private actionServise: ActionsService, private router: Router) {}

  ngOnInit(): void {}

  setDocumentType(type: DocumentType) {
    this.type = type;
  }

  sendEnvelope(type: DocumentType) {
    this.actionServise
      .sendEnvelope(type, null, this.redirectUrl)
      .subscribe((payload) => {
        window.location.href = payload.redirectUrl;
      });
  }

  submit() {
    this.actionServise
      .sendEnvelope(this.type, this.additionalUserForm.value, this.redirectUrl)
      .subscribe((payload) => {
        window.location.href = payload.redirectUrl;
      });
  }

  sendTimeCard(type: string) {
    this.router.navigate(["/employee/timecard"]);
  }
  doAction() {}
}
