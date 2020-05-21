import { Component, OnInit } from "@angular/core";
import { ActionsService } from "./actions.service";
import { DocumentType } from "./document-type.enum";
import { FormControl, Validators, FormGroup, FormArray } from "@angular/forms";
import { DOCUMENT } from "@angular/common";
import { Renderer2, Inject } from "@angular/core";

declare const window: Window & { docuSignClick: any };

@Component({
  selector: "app-actions",
  templateUrl: "./actions.component.html",
  styleUrls: ["./actions.component.css"],
})
export class ActionsComponent implements OnInit {
  public documentType = DocumentType;
  private readonly scriptUrl =
    "//demo.docusign.net/clickapi/sdk/latest/docusign-click.js";
  private total: number;
  private period: Date;
  private isLoadedClickWrap: boolean;

  weekDays: string[] = [
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday",
  ];

  timecardForm: FormGroup = new FormGroup({
    workLogs: new FormArray(
      Object.keys(this.weekDays).map(
        () => new FormControl("", Validators.required)
      )
    ),
  });

  constructor(
    private actionServise: ActionsService,
    private renderer2: Renderer2,
    @Inject(DOCUMENT) private document: Document
  ) {}

  ngOnInit(): void {}

  sendEnvelope(type: DocumentType) {
    this.actionServise.sendEnvelope(type, null).subscribe((payload) => {
      window.location.href = payload.redirectUrl;
    });
  }

  sendTimeCard(type: string) {
    this.actionServise
      .createClickWrap(this.timecardForm.value.workLogs)
      .subscribe((payload) => {
        const clickwrap = payload.clickWrap;
        const baseUrl = payload.docuSignBaseUrl;
        this.loadClickWrap(clickwrap, baseUrl);
        if (this.isLoadedClickWrap) {
        }
      });
  }

  loadClickWrap(clickwrap: any, baseUrl: string) {
    const existingScript = document.getElementById("clickwrapscript");
    if (existingScript) {
      this.showClickWrap(clickwrap, baseUrl);
    } else {
      const textScript = this.renderer2.createElement("script");
      textScript.src = this.scriptUrl;
      textScript.id = "clickwrapscript";
      this.renderer2.appendChild(this.document.body, textScript);

      textScript.onload = () => {
        this.isLoadedClickWrap = true;
        this.showClickWrap(clickwrap, baseUrl);
      };
    }
  }

  showClickWrap(clickwrap: any, baseUrl: string) {
    window.docuSignClick.Clickwrap.render(
      {
        environment: baseUrl,
        accountId: clickwrap.accountId,
        clickwrapId: clickwrap.clickwrapId,
        clientUserId: clickwrap.accountId,
      },
      "#ds-clickwrap"
    );
  }
}
